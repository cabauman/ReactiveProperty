﻿using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using Reactive.Todo.Main.Events;
using Reactive.Todo.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Todo.Main.ViewModels
{
    public class CommandsViewModel : ViewModelBase
    {
        public ReadOnlyReactivePropertySlim<TargetViewType> TargetViewType { get; }
        public ReactiveCommand<TargetViewType> ChangeTargetViewTypeCommand { get; }
        public ReadOnlyReactivePropertySlim<int> CountOfItemsLeft { get; }
        public ReactiveCommand ClearCompletedCommand { get; }
        public CommandsViewModel(TodoApp todoApp, IMessageBroker messageBroker)
        {
            ChangeTargetViewTypeCommand = new ReactiveCommand<TargetViewType>()
                .WithSubscribe(x => messageBroker.Publish(new TargetViewChangedEvent(x)))
                .AddTo(Disposables);

            TargetViewType = ChangeTargetViewTypeCommand.ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
            CountOfItemsLeft = todoApp.AllTodoItems
                .ObserveElementProperty(x => x.Completed)
                .Select(_ => todoApp.ActiveTodoItems.Count)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ClearCompletedCommand = new ReactiveCommand()
                .WithSubscribe(() => todoApp.ClearCompletedItems())
                .AddTo(Disposables);
        }
    }
}
