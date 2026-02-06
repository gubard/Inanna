using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Models;
using Inanna.Services;
using Nestor.Db.Models;

namespace Inanna.Controls;

public abstract class DropUserControl<
    TUiService,
    TGetRequest,
    TPostRequest,
    TGetResponse,
    TPostResponse,
    TEdit
> : UserControl
    where TUiService : IUiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IResponse, new()
    where TPostResponse : IPostResponse, new()
    where TPostRequest : IDragChangeOrder<TEdit>, new()
    where TEdit : IEdit, new()
{
    protected DropUserControl()
    {
        _uiService = DiHelper.ServiceProvider.GetService<TUiService>();
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private readonly ReadOnlyMemory<string> _dropTags = new[]
    {
        "DropRoot",
        "DropUp",
        "DropDown",
        "DropParent",
    };

    private readonly TUiService _uiService;

    private void DragOver(object? sender, DragEventArgs e)
    {
        var tag = FindObjectDropTag(e.Source);

        if (tag is not null && _dropTags.Span.Contains(tag))
        {
            e.DragEffects &= DragDropEffects.Move;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private string? FindObjectDropTag(object? obj)
    {
        if (obj is null)
        {
            return null;
        }

        if (obj is Panel panel)
        {
            return panel.Tag?.ToString();
        }

        return obj.As<Visual>()?.GetVisualParent<Panel>()?.Tag?.ToString();
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var tag = FindObjectDropTag(e.Source);
        var data = e
            .DataTransfer.Items[0]
            .TryGetRaw(e.DataTransfer.Items[0].Formats[0])
            .As<byte[]>();

        if (data is null)
        {
            return;
        }

        var id = new Guid(data);

        switch (tag)
        {
            case "DropRoot":
            {
                _uiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        Edits =
                        [
                            new()
                            {
                                Ids = [id],
                                IsEditParentId = true,
                                ParentId = null,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropParent":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<IIsDrag>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        Edits =
                        [
                            new()
                            {
                                Ids = [id],
                                IsEditParentId = true,
                                ParentId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropUp":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<IIsDrag>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        ChangeOrders =
                        [
                            new()
                            {
                                InsertIds = [id],
                                IsAfter = false,
                                StartId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropDown":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<IIsDrag>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        ChangeOrders =
                        [
                            new()
                            {
                                InsertIds = [id],
                                IsAfter = true,
                                StartId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
        }
    }
}
