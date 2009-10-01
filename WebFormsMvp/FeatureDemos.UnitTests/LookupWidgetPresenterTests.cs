﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebFormsMvp.FeatureDemos.Logic.Presenters;
using Rhino.Mocks;
using WebFormsMvp.FeatureDemos.Logic.Views;
using WebFormsMvp.FeatureDemos.Logic.Views.Models;
using WebFormsMvp.FeatureDemos.Logic.Data;
using Domain = WebFormsMvp.FeatureDemos.Logic.Domain;
using WebFormsMvp.Testing;
using System.Threading;
using Rhino.Mocks.Interfaces;

namespace WebFormsMvp.FeatureDemos.UnitTests
{
    [TestClass]
    public class LookupWidgetPresenterTests
    {
        [TestMethod]
        public void LookupWidgetPresenterLoadsWidgetFromId()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var asyncManager = new TestAsyncTaskManager();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();
            var widget = new Widget() { Id = 1, Name = "Test" };

            widgetRepository.Stub(w => w.BeginFind(1, null, null)).IgnoreArguments()
                .ExecuteAsyncCallback().Return(null);
            widgetRepository.Stub(w => w.EndFind(null)).IgnoreArguments()
                .Return(widget);

            var presenter = new LookupWidgetPresenter(view, widgetRepository);
            presenter.AsyncManager = asyncManager;

            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            view.Raise(v => v.Finding += null, view, new FindingWidgetEventArgs() { Id = 1 });
            asyncManager.ExecuteRegisteredAsyncTasks(); // Execute the tasks here as ASP.NET would normally do for us
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(widget, view.Model.Widgets.First());
        }

        [TestMethod]
        public void LookupWidgetPresenterLoadsWidgetFromIdWhenBothIdAndNameSet()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var asyncManager = new TestAsyncTaskManager();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();
            var widget = new Widget() { Id = 1, Name = "Test" };

            widgetRepository.Stub(w => w.BeginFind(1, null, null)).IgnoreArguments()
                .ExecuteAsyncCallback().Return(null);
            widgetRepository.Stub(w => w.EndFind(null)).IgnoreArguments()
                .Return(widget);

            var presenter = new LookupWidgetPresenter(view, widgetRepository);
            presenter.AsyncManager = asyncManager;

            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            view.Raise(v => v.Finding += null, view, new FindingWidgetEventArgs() { Id = 1, Name = "Blah" });
            asyncManager.ExecuteRegisteredAsyncTasks(); // Execute the tasks here as ASP.NET would normally do for us
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(widget, view.Model.Widgets.First());
        }

        [TestMethod]
        public void LookupWidgetPresenterLoadsWidgetFromName()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var asyncManager = new TestAsyncTaskManager();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();
            var widget = new Widget() { Id = 1, Name = "Test" };

            widgetRepository.Stub(w => w.BeginFindByName("Test", null, null)).IgnoreArguments()
                .ExecuteAsyncCallback().Return(null);
            widgetRepository.Stub(w => w.EndFindByName(null)).IgnoreArguments()
                .Return(widget);

            var presenter = new LookupWidgetPresenter(view, widgetRepository);
            presenter.AsyncManager = asyncManager;

            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            view.Raise(v => v.Finding += null, view, new FindingWidgetEventArgs() { Name = "Test" });
            asyncManager.ExecuteRegisteredAsyncTasks(); // Execute the tasks here as ASP.NET would normally do for us
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(widget, view.Model.Widgets.First());
        }

        [TestMethod]
        public void LookupWidgetPresenterLoadsWidgetFromNameWhenBothIdAndNameSetButIdIsInvalid()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var asyncManager = new TestAsyncTaskManager();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();
            var widget = new Widget() { Id = 1, Name = "Test" };

            widgetRepository.Stub(w => w.BeginFindByName("Test", null, null)).IgnoreArguments()
                .ExecuteAsyncCallback().Return(null);
            widgetRepository.Stub(w => w.EndFindByName(null)).IgnoreArguments()
                .Return(widget);

            var presenter = new LookupWidgetPresenter(view, widgetRepository);
            presenter.AsyncManager = asyncManager;

            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            view.Raise(v => v.Finding += null, view, new FindingWidgetEventArgs() { Id = -1, Name = "Test" });
            asyncManager.ExecuteRegisteredAsyncTasks(); // Execute the tasks here as ASP.NET would normally do for us
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(widget, view.Model.Widgets.First());
        }

        [TestMethod]
        public void LookupWidgetPresenterHidesResultsOnInitialLoad()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();

            var presenter = new LookupWidgetPresenter(view, widgetRepository);

            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(false, view.Model.ShowResults);
        }

        [TestMethod]
        public void LookupWidgetPresenterShowsResultsOnFinding()
        {
            // Arrange
            var view = MockRepository.GenerateStub<ILookupWidgetView>();
            var asyncManager = new TestAsyncTaskManager();
            var widgetRepository = MockRepository.GenerateStub<IWidgetRepository>();
            var widget = new Widget() { Id = 1, Name = "Test" };

            widgetRepository.Stub(w => w.BeginFindByName("Test", null, null)).IgnoreArguments()
                .ExecuteAsyncCallback().Return(null);
            widgetRepository.Stub(w => w.EndFindByName(null)).IgnoreArguments()
                .Return(widget);

            var presenter = new LookupWidgetPresenter(view, widgetRepository);
            presenter.AsyncManager = asyncManager;
            
            // Act
            view.Raise(v => v.Load += null, view, new EventArgs());
            view.Raise(v => v.Finding += null, view, new FindingWidgetEventArgs() { Name = "Test" });
            presenter.ReleaseView();

            // Assert
            Assert.AreEqual(true, view.Model.ShowResults);
        }
    }
}