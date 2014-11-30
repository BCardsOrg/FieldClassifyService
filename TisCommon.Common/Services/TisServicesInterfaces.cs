using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using TiS.Core.TisCommon.Customizations;
using TiS.Core.TisCommon.Validation;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Services
{
    #region ITisServicesSchemaCollection

    [ComVisible(false)]
    public interface ITisServicesSchemaCollection
    {
        List<ITisServiceInfo> GetServices();
    }

    #endregion

    #region ITisServiceRegistry

    public interface ITisServiceRegistry
    {
        // Install
        void InstallService(
            string sServiceName,	    // Optional if sServiceType specified
            string sServiceCreatorType,
            string sServiceImplType,
            string[] RequiredRoles,
            bool freeUse
            );

        // Uninstall
        void UninstallService(string sServiceName);

        // Installed services information
        ITisServiceInfo[] InstalledServices { get; }

        int NumberOfInstalledServices { get; }

        ITisServiceInfo GetInstalledServiceInfo(string sServiceName);

        bool IsServiceInstalled(string sServiceName);

        string ServicesSchema { get; set; }
        Type ServicesSchemaCLRType { get; set; }

        bool ReadOnly { get; set; }
    }

    #endregion

    #region ITisServiceRegistryProvider

    public interface ITisServiceRegistryProvider
    {
        ITisServiceRegistry GetServiceRegistry(string sAppName);
    }

    #endregion

    #region ITisServicesHost

    public delegate void TisServiceEventDelegate(
        string sAppName,
        TisServiceKey oServiceKey);

    public interface ITisServicesHost :
        ITisServiceRegistryProvider,
        IDisposable
    {
        #region Events

        event TisServiceEventDelegate OnPreServiceActivate;
        event TisServiceEventDelegate OnPostServiceActivate;
        event TisServiceEventDelegate OnPreServiceDeactivate;
        event TisServiceEventDelegate OnPostServiceDeactivate;

        #endregion

        string Name { get; }

        BasicConfiguration BasicConfiguration { get; }

        #region GetService

        // Full location specification

        object GetService(
            string sHostName,
            string sAppName,
            string sServiceName,
            string sServiceInstanceName);

        object GetService(
            string sHostName,
            string sAppName,
            Type oServiceType,
            string sServiceInstanceName);

        // HostName not specified

        object GetService(
            string sAppName,
            string sServiceName);

        object GetService(
            string sAppName,
            Type oServiceType);

        object GetService(
            string sAppName,
            Type oServiceType,
            string sServiceInstanceName);

        object GetService(
            string sAppName,
            string sServiceName,
            string sServiceInstanceName);

        #endregion

        #region GetSystemService

        object GetSystemService(
            string sServiceName,
            string sServiceInstanceName);

        object GetSystemService(
            Type oServiceType,
            string sServiceInstanceName);

        object GetSystemService(string sServiceName);

        object GetSystemService(Type oServiceType);

        #endregion

        #region CanHostService

        bool CanHostService(
            string sAppName,
            string sServiceName);

        bool CanHostService(
            string sAppName,
            ITisServiceInfo oServiceInfo);

        #endregion

        #region GetServiceInfo

        ITisServiceInfo GetServiceInfo(
            string sAppName,
            string sServiceName);

        ITisServiceInfo CheckedGetServiceInfo(
            string sAppName,
            string sServiceName);

        #endregion

        #region IsServiceInstalled

        bool IsServiceInstalled(
            string sAppName,
            string sServiceName);

        bool IsServiceInstalled(
            string sAppName,
            Type oServiceType);

        #endregion

        #region StopApplicationServices

        void StopApplicationServices(
            string sAppName);

        #endregion

        string GetBuiltInServicesSchema(string sAppName);
    }

    #endregion

    #region ITisServiceProvider

    public interface ITisServiceProvider : IDisposable
    {
        object GetService(
            TisServiceKey oServiceKey);

        void ReleaseService(
            TisServiceKey oServiceKey,
            ITisServiceInfo serviceInfo = null,
            object oService = null);

        void AddService(
            TisServiceKey oServiceKey,
            object oService);

        bool IsInstantiated(TisServiceKey oServiceKey);
    }

    #endregion

    #region ITisServiceLifetimeManager

    public delegate void TisServiceLifetimeEvent(
        string sAppName,
        TisServiceKey oServiceKey);

    public delegate void TisServiceLifetimeEventEx(
        string sAppName,
        TisServiceKey oServiceKey,
        object oService);

    public interface ITisServiceLifetimeManager
    {
        event TisServiceLifetimeEvent OnPreServiceActivate;
        event TisServiceLifetimeEventEx OnPostServiceActivate;
        event TisServiceLifetimeEventEx OnPreServiceDeactivate;
        event TisServiceLifetimeEvent OnPostServiceDeactivate;
    }

    #endregion

    #region ITisServiceCreator

    public interface ITisServiceCreator : IDisposable
    {
        object CreateService();
        void ReleaseService(object oService);
    }

    #endregion

    #region ITisServiceCreatorContextSetter

    public interface ITisServiceCreatorContextSetter
    {
        ITisServiceCreatorContextSetter Next { get; set; }

        void SetCreatorContext(
            ITisServiceCreator oCreator,
            string sAppName,
            TisServiceKey oServiceKey,
            ITisServicesHost oServicesHost);
    }

    internal interface ISupportsCreatorContext
    {
        void SetContext(
            string sAppName,
            TisServiceKey oServiceKey,
            ITisServicesHost oServicesHost);
    }

    #endregion

    #region IPersistKeyProvider

    public interface IPersistKeyProvider
    {
        string TypedPersistKey { get; }
        string FullPersistKey { get; }
    }

    #endregion

    #region ITisServiceInfo

    public enum ServicesUsingMode { Restricted, Free };

    public interface ITisServiceInfo
    {
        string ServiceCreatorType { get; }
        string ServiceName { get; }
        string ServiceImplType { get; }
        string[] RequiredRoles { get; }
        string[] SupportedPermissions { get; }
        bool FromSchema { get; }
        string Alias { get; }
        ServicesUsingMode UsingMode { get; }
        List<TisEventParams> SupportedEvents { get; }
        List<TisValidationMethod> SupportedValidations { get; }
        TisStationDeclarationInfo StationDeclarationInfo { get; }
    }

    #endregion

    #region IAppServiceProvider

    [Guid("c70a246c-d1dd-4ec8-941b-405329b2ce42")]
    public interface IAppServiceProvider : IDisposable
    {
        object GetService(string sServiceName);
        object GetService(Type oServiceType);
        object GetService(string sServiceName, string sInstanceName);
        object GetService(Type oServiceType, string sInstanceName);
        ITisServiceRegistry GetServiceRegistry();
    }

    #endregion
}
