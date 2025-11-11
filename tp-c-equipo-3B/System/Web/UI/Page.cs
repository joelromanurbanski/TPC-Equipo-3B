#region ensamblado System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// ubicación desconocida
// Decompiled with ICSharpCode.Decompiler 8.2.0.7535
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.EnterpriseServices;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Management;
using System.Web.ModelBinding;
using System.Web.RegularExpressions;
using System.Web.Routing;
using System.Web.Security.Cryptography;
using System.Web.SessionState;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Util;
using System.Xml;

namespace System.Web.UI;

//
// Resumen:
//     Representa un archivo .aspx, también conocido como una página de formularios
//     Web Forms, solicitado desde un servidor que hospeda una aplicación web ASP.NET.
[DefaultEvent("Load")]
[Designer("Microsoft.VisualStudio.Web.WebForms.WebFormDesigner, Microsoft.VisualStudio.Web, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
[DesignerCategory("ASPXCodeBehind")]
[DesignerSerializer("Microsoft.VisualStudio.Web.WebForms.WebFormCodeDomSerializer, Microsoft.VisualStudio.Web, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.TypeCodeDomSerializer, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ToolboxItem(false)]
public class Page : TemplateControl, IHttpHandler
{
    private class LegacyPageAsyncInfo
    {
        private Page _page;

        private bool _callerIsBlocking;

        private HttpApplication _app;

        private AspNetSynchronizationContextBase _syncContext;

        private HttpAsyncResult _asyncResult;

        private bool _asyncPointReached;

        private int _handlerCount;

        private ArrayList _beginHandlers;

        private ArrayList _endHandlers;

        private ArrayList _stateObjects;

        private AsyncCallback _completionCallback;

        private WaitCallback _callHandlersThreadpoolCallback;

        private int _currentHandler;

        private Exception _error;

        private bool _completed;

        internal HttpAsyncResult AsyncResult
        {
            get
            {
                return _asyncResult;
            }
            set
            {
                _asyncResult = value;
            }
        }

        internal bool AsyncPointReached
        {
            get
            {
                return _asyncPointReached;
            }
            set
            {
                _asyncPointReached = value;
            }
        }

        internal bool CallerIsBlocking
        {
            get
            {
                return _callerIsBlocking;
            }
            set
            {
                _callerIsBlocking = value;
            }
        }

        internal LegacyPageAsyncInfo(Page page)
        {
            _page = page;
            _app = page.Context.ApplicationInstance;
            _syncContext = page.Context.SyncContext;
            _completionCallback = OnAsyncHandlerCompletion;
            _callHandlersThreadpoolCallback = CallHandlersFromThreadpoolThread;
        }

        internal void AddHandler(BeginEventHandler beginHandler, EndEventHandler endHandler, object state)
        {
            if (_handlerCount == 0)
            {
                _beginHandlers = new ArrayList();
                _endHandlers = new ArrayList();
                _stateObjects = new ArrayList();
            }

            _beginHandlers.Add(beginHandler);
            _endHandlers.Add(endHandler);
            _stateObjects.Add(state);
            _handlerCount++;
        }

        internal void CallHandlers(bool onPageThread)
        {
            try
            {
                if (CallerIsBlocking || onPageThread)
                {
                    CallHandlersPossiblyUnderLock(onPageThread);
                    return;
                }

                lock (_app)
                {
                    CallHandlersPossiblyUnderLock(onPageThread);
                }
            }
            catch (Exception error)
            {
                Exception ex = (_error = error);
                _completed = true;
                _asyncResult.Complete(onPageThread, null, _error);
                if (!onPageThread && ex is ThreadAbortException && ((ThreadAbortException)ex).ExceptionState is HttpApplication.CancelModuleException)
                {
                    ThreadResetAbortWithAssert();
                }
            }
        }

        private void CallHandlersPossiblyUnderLock(bool onPageThread)
        {
            ThreadContext threadContext = null;
            if (!onPageThread)
            {
                threadContext = _app.OnThreadEnter();
            }

            try
            {
                while (_currentHandler < _handlerCount && _error == null)
                {
                    try
                    {
                        IAsyncResult asyncResult = ((BeginEventHandler)_beginHandlers[_currentHandler])(_page, EventArgs.Empty, _completionCallback, _stateObjects[_currentHandler]);
                        if (asyncResult == null)
                        {
                            throw new InvalidOperationException(SR.GetString("Async_null_asyncresult"));
                        }

                        if (asyncResult.CompletedSynchronously)
                        {
                            try
                            {
                                ((EndEventHandler)_endHandlers[_currentHandler])(asyncResult);
                            }
                            finally
                            {
                                _currentHandler++;
                            }

                            continue;
                        }

                        return;
                    }
                    catch (Exception ex)
                    {
                        if (onPageThread && _syncContext.PendingOperationsCount == 0)
                        {
                            throw;
                        }

                        PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_DURING_REQUEST);
                        PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
                        try
                        {
                            if (!_page.HandleError(ex))
                            {
                                _error = ex;
                            }
                        }
                        catch (Exception error)
                        {
                            _error = error;
                        }
                    }
                }

                if (_syncContext.PendingCompletion(_callHandlersThreadpoolCallback))
                {
                    return;
                }

                if (_error == null && _syncContext.Error != null)
                {
                    try
                    {
                        if (!_page.HandleError(_syncContext.Error))
                        {
                            _error = _syncContext.Error;
                            _syncContext.ClearError();
                        }
                    }
                    catch (Exception error2)
                    {
                        _error = error2;
                    }
                }

                try
                {
                    _page.Context.InvokeCancellableCallback(delegate
                    {
                        _page.ProcessRequest(includeStagesBeforeAsyncPoint: false, includeStagesAfterAsyncPoint: true);
                    }, null);
                }
                catch (Exception error3)
                {
                    if (onPageThread)
                    {
                        throw;
                    }

                    _error = error3;
                }

                if (threadContext != null)
                {
                    try
                    {
                        threadContext.DisassociateFromCurrentThread();
                    }
                    finally
                    {
                        threadContext = null;
                    }
                }

                _completed = true;
                _asyncResult.Complete(onPageThread, null, _error);
            }
            finally
            {
                threadContext?.DisassociateFromCurrentThread();
            }
        }

        private void OnAsyncHandlerCompletion(IAsyncResult ar)
        {
            if (ar.CompletedSynchronously)
            {
                return;
            }

            try
            {
                ((EndEventHandler)_endHandlers[_currentHandler])(ar);
            }
            catch (Exception error)
            {
                _error = error;
            }

            if (!_completed)
            {
                _currentHandler++;
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    CallHandlers(onPageThread: false);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(_callHandlersThreadpoolCallback);
                }
            }
        }

        private void CallHandlersFromThreadpoolThread(object data)
        {
            CallHandlers(onPageThread: false);
        }

        internal void SetError(Exception error)
        {
            _error = error;
        }
    }

    private const string HiddenClassName = "aspNetHidden";

    private const string PageID = "__Page";

    private const string PageScrollPositionScriptKey = "PageScrollPositionScript";

    private const string PageSubmitScriptKey = "PageSubmitScript";

    private const string PageReEnableControlsScriptKey = "PageReEnableControlsScript";

    private const string PageRegisteredControlsThatRequirePostBackKey = "__ControlsRequirePostBackKey__";

    private const string EnabledControlArray = "__enabledControlArray";

    internal static readonly object EventPreRenderComplete;

    internal static readonly object EventPreLoad;

    internal static readonly object EventLoadComplete;

    internal static readonly object EventPreInit;

    internal static readonly object EventInitComplete;

    internal static readonly object EventSaveStateComplete;

    private static readonly Version FocusMinimumEcmaVersion;

    private static readonly Version FocusMinimumJScriptVersion;

    private static readonly Version JavascriptMinimumVersion;

    private static readonly Version MSDomScrollMinimumVersion;

    private static readonly string UniqueFilePathSuffixID;

    private string _uniqueFilePathSuffix;

    internal static readonly int DefaultMaxPageStateFieldLength;

    internal static readonly int DefaultAsyncTimeoutSeconds;

    private int _maxPageStateFieldLength = DefaultMaxPageStateFieldLength;

    private string _requestViewState;

    private bool _cachedRequestViewState;

    private PageAdapter _pageAdapter;

    private bool _fPageLayoutChanged;

    private bool _haveIdSeparator;

    private char _idSeparator;

    private bool _sessionRetrieved;

    private HttpSessionState _session;

    private int _transactionMode;

    private bool _aspCompatMode;

    private bool _asyncMode;

    private static readonly TimeSpan _maxAsyncTimeout;

    private TimeSpan _asyncTimeout;

    private bool _asyncTimeoutSet;

    private PageAsyncTaskManager _asyncTaskManager;

    private LegacyPageAsyncTaskManager _legacyAsyncTaskManager;

    private LegacyPageAsyncInfo _legacyAsyncInfo;

    private CultureInfo _dynamicCulture;

    private CultureInfo _dynamicUICulture;

    private string _clientState;

    private PageStatePersister _persister;

    internal ControlSet _registeredControlsRequiringControlState;

    private StringSet _controlStateLoadedControlIds;

    internal HybridDictionary _registeredControlsRequiringClearChildControlState;

    internal const ViewStateEncryptionMode EncryptionModeDefault = ViewStateEncryptionMode.Auto;

    private ViewStateEncryptionMode _encryptionMode;

    private bool _viewStateEncryptionRequested;

    private ArrayList _enabledControls;

    internal HttpRequest _request;

    internal HttpResponse _response;

    internal HttpApplicationState _application;

    internal Cache _cache;

    internal string _errorPage;

    private string _clientTarget;

    private HtmlForm _form;

    private bool _inOnFormRender;

    private bool _fOnFormRenderCalled;

    private bool _fRequireWebFormsScript;

    private bool _fWebFormsScriptRendered;

    private bool _fRequirePostBackScript;

    private bool _fPostBackScriptRendered;

    private bool _containsCrossPagePost;

    private RenderMethod _postFormRenderDelegate;

    internal Dictionary<string, string> _hiddenFieldsToRender;

    private bool _requireFocusScript;

    private bool _profileTreeBuilt;

    internal const bool MaintainScrollPositionOnPostBackDefault = false;

    private bool _maintainScrollPosition;

    private ClientScriptManager _clientScriptManager;

    private static Type _scriptManagerType;

    internal const bool EnableViewStateMacDefault = true;

    internal const bool EnableEventValidationDefault = true;

    internal const string systemPostFieldPrefix = "__";

    //
    // Resumen:
    //     Una cadena que define el campo oculto EVENTTARGET de la página representada.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string postEventSourceID = "__EVENTTARGET";

    private const string lastFocusID = "__LASTFOCUS";

    private const string _scrollPositionXID = "__SCROLLPOSITIONX";

    private const string _scrollPositionYID = "__SCROLLPOSITIONY";

    //
    // Resumen:
    //     Una cadena que define el campo oculto EVENTARGUMENT de la página representada.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string postEventArgumentID = "__EVENTARGUMENT";

    internal const string ViewStateFieldPrefixID = "__VIEWSTATE";

    internal const string ViewStateFieldCountID = "__VIEWSTATEFIELDCOUNT";

    internal const string ViewStateGeneratorFieldID = "__VIEWSTATEGENERATOR";

    internal const string ViewStateEncryptionID = "__VIEWSTATEENCRYPTED";

    internal const string EventValidationPrefixID = "__EVENTVALIDATION";

    internal const string WebPartExportID = "__WEBPARTEXPORT";

    private bool _requireScrollScript;

    private bool _isCallback;

    private bool _isCrossPagePostBack;

    private bool _containsEncryptedViewState;

    private bool _enableEventValidation = true;

    internal const string callbackID = "__CALLBACKID";

    internal const string callbackParameterID = "__CALLBACKPARAM";

    internal const string callbackLoadScriptID = "__CALLBACKLOADSCRIPT";

    internal const string callbackIndexID = "__CALLBACKINDEX";

    internal const string previousPageID = "__PREVIOUSPAGE";

    private Stack _partialCachingControlStack;

    private ArrayList _controlsRequiringPostBack;

    private ArrayList _registeredControlsThatRequirePostBack;

    private NameValueCollection _leftoverPostData;

    private IPostBackEventHandler _registeredControlThatRequireRaiseEvent;

    private ArrayList _changedPostDataConsumers;

    private bool _needToPersistViewState;

    private bool _enableViewStateMac;

    private string _viewStateUserKey;

    private string _themeName;

    private PageTheme _theme;

    private string _styleSheetName;

    private PageTheme _styleSheet;

    private VirtualPath _masterPageFile;

    private MasterPage _master;

    private IDictionary _contentTemplateCollection;

    private SmartNavigationSupport _smartNavSupport;

    internal HttpContext _context;

    private ValidatorCollection _validators;

    private bool _validated;

    private HtmlHead _header;

    private int _supportsStyleSheets;

    private Control _autoPostBackControl;

    private string _focusedControlID;

    private Control _focusedControl;

    private string _validatorInvalidControl;

    private int _scrollPositionX;

    private int _scrollPositionY;

    private Page _previousPage;

    private VirtualPath _previousPagePath;

    private bool _preInitWorkComplete;

    private bool _clientSupportsJavaScriptChecked;

    private bool _clientSupportsJavaScript;

    private string _titleToBeSet;

    private string _descriptionToBeSet;

    private string _keywordsToBeSet;

    private ICallbackEventHandler _callbackControl;

    private bool _xhtmlConformanceModeSet;

    private XhtmlConformanceMode _xhtmlConformanceMode;

    private const int styleSheetInitialized = 1;

    private const int isExportingWebPart = 2;

    private const int isExportingWebPartShared = 4;

    private const int isCrossPagePostRequest = 8;

    private const int isPartialRenderingSupported = 16;

    private const int isPartialRenderingSupportedSet = 32;

    private const int skipFormActionValidation = 64;

    private const int wasViewStateMacErrorSuppressed = 128;

    private SimpleBitVector32 _pageFlags;

    private NameValueCollection _requestValueCollection;

    private NameValueCollection _unvalidatedRequestValueCollection;

    private ModelStateDictionary _modelState;

    private ModelBindingExecutionContext _modelBindingExecutionContext;

    private UnobtrusiveValidationMode? _unobtrusiveValidationMode;

    private bool _executingAsyncTasks;

    private static StringSet s_systemPostFields;

    private string _clientQueryString;

    private static char[] s_varySeparator;

    internal const bool BufferDefault = true;

    internal const bool SmartNavigationDefault = false;

    private AspCompatApplicationStep _aspCompatStep;

    private string _relativeFilePath;

    private bool _designModeChecked;

    private bool _designMode;

    private IDictionary _items;

    private Stack _dataBindingContext;

    //
    // Resumen:
    //     Obtiene el objeto de diccionario de estado de modelo que contiene el estado del
    //     modelo y de validación de enlace de modelos.
    //
    // Devuelve:
    //     Objeto de diccionario de Estados del modelo.
    public ModelStateDictionary ModelState
    {
        get
        {
            if (_modelState == null)
            {
                _modelState = new ModelStateDictionary();
            }

            return _modelState;
        }
    }

    private IValueProvider ActiveValueProvider { get; set; }

    internal bool IsExecutingAsyncTasks
    {
        get
        {
            return _executingAsyncTasks;
        }
        set
        {
            _executingAsyncTasks = value;
        }
    }

    //
    // Resumen:
    //     Obtiene el contexto de ejecución del enlace de modelo.
    //
    // Devuelve:
    //     El contexto de ejecución del enlace de modelo. Si el contexto de ejecución del
    //     enlace de modelo es null, un nuevo uno se crea y se devuelve.
    public ModelBindingExecutionContext ModelBindingExecutionContext
    {
        get
        {
            if (_modelBindingExecutionContext == null)
            {
                _modelBindingExecutionContext = new ModelBindingExecutionContext(new HttpContextWrapper(Context), ModelState);
                _modelBindingExecutionContext.PublishService(ViewState);
                _modelBindingExecutionContext.PublishService(RouteData);
            }

            return _modelBindingExecutionContext;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si se usa JavaScript discreto para la
    //     validación del lado cliente.
    //
    // Devuelve:
    //     true Si se usa JavaScript discreto; de lo contrario, false.
    [DefaultValue(UnobtrusiveValidationMode.None)]
    [WebCategory("Behavior")]
    [WebSysDescription("Page_UnobtrusiveValidationMode")]
    public UnobtrusiveValidationMode UnobtrusiveValidationMode
    {
        get
        {
            return _unobtrusiveValidationMode ?? ValidationSettings.UnobtrusiveValidationMode;
        }
        set
        {
            if (value < UnobtrusiveValidationMode.None || value > UnobtrusiveValidationMode.WebForms)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            _unobtrusiveValidationMode = value;
        }
    }

    //
    // Resumen:
    //     Obtiene el System.Web.HttpApplicationState objeto de la solicitud Web actual.
    //
    //
    // Devuelve:
    //     Los datos actuales de la System.Web.HttpApplicationState clase.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HttpApplicationState Application => _application;

    //
    // Resumen:
    //     Obtiene el System.Web.HttpContext objeto asociado a la página.
    //
    // Devuelve:
    //     Un System.Web.HttpContext objeto que contiene información asociada a la página
    //     actual.
    protected internal override HttpContext Context
    {
        get
        {
            if (_context == null)
            {
                _context = HttpContext.Current;
            }

            return _context;
        }
    }

    private StringSet ControlStateLoadedControlIds
    {
        get
        {
            if (_controlStateLoadedControlIds == null)
            {
                _controlStateLoadedControlIds = new StringSet();
            }

            return _controlStateLoadedControlIds;
        }
    }

    internal string ClientState
    {
        get
        {
            return _clientState;
        }
        set
        {
            _clientState = value;
        }
    }

    internal string ClientOnSubmitEvent
    {
        get
        {
            if (ClientScript.HasSubmitStatements || (Form != null && Form.SubmitDisabledControls && EnabledControls.Count > 0))
            {
                return "javascript:return WebForm_OnSubmit();";
            }

            return string.Empty;
        }
    }

    //
    // Resumen:
    //     Obtiene un System.Web.UI.ClientScriptManager objeto se utiliza para administrar,
    //     registrar y agregar script a la página.
    //
    // Devuelve:
    //     Objeto System.Web.UI.ClientScriptManager.
    public ClientScriptManager ClientScript
    {
        get
        {
            if (_clientScriptManager == null)
            {
                _clientScriptManager = new ClientScriptManager(this);
            }

            return _clientScriptManager;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que permite reemplazar la detección automática de
    //     funciones del explorador y especificar cómo se representa una página para clientes
    //     de explorador concretos.
    //
    // Devuelve:
    //     Un System.String que especifica las capacidades del explorador que se desean
    //     reemplazar.
    [DefaultValue("")]
    [WebSysDescription("Page_ClientTarget")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ClientTarget
    {
        get
        {
            if (_clientTarget != null)
            {
                return _clientTarget;
            }

            return string.Empty;
        }
        set
        {
            _clientTarget = value;
            if (_request != null)
            {
                _request.ClientTarget = value;
            }
        }
    }

    //
    // Resumen:
    //     Obtiene la parte de la cadena de consulta de la dirección URL solicitada.
    //
    // Devuelve:
    //     La parte de la cadena de consulta de la dirección URL solicitada.
    public string ClientQueryString
    {
        get
        {
            if (_clientQueryString == null)
            {
                if (RequestInternal != null && Request.HasQueryString)
                {
                    Hashtable hashtable = new Hashtable();
                    foreach (string item in (IEnumerable)s_systemPostFields)
                    {
                        hashtable.Add(item, true);
                    }

                    HttpValueCollection httpValueCollection = (HttpValueCollection)(SkipFormActionValidation ? Request.Unvalidated.QueryString : Request.QueryString);
                    _clientQueryString = httpValueCollection.ToString(urlencoded: true, hashtable);
                }
                else
                {
                    _clientQueryString = string.Empty;
                }
            }

            return _clientQueryString;
        }
    }

    internal bool ContainsEncryptedViewState
    {
        get
        {
            return _containsEncryptedViewState;
        }
        set
        {
            _containsEncryptedViewState = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece la página de error a la que se redirige el explorador solicitante
    //     si se produce una excepción de página no controlada.
    //
    // Devuelve:
    //     La página de error a la que se redirige el explorador.
    [DefaultValue("")]
    [WebSysDescription("Page_ErrorPage")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ErrorPage
    {
        get
        {
            return _errorPage;
        }
        set
        {
            _errorPage = value;
        }
    }

    //
    // Resumen:
    //     Obtiene un valor que indica si la solicitud de página es el resultado de una
    //     devolución de llamada.
    //
    // Devuelve:
    //     true Si la solicitud de página es el resultado de una devolución de llamada;
    //     de lo contrario, false.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsCallback => _isCallback;

    //
    // Resumen:
    //     Obtiene un valor que indica si el System.Web.UI.Page se puede reutilizar el objeto.
    //
    //
    // Devuelve:
    //     false en todos los casos.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsReusable => false;

    //
    // Resumen:
    //     Obtiene un sufijo único que se va a anexar a la ruta de acceso de archivo para
    //     los exploradores de almacenamiento en caché.
    //
    // Devuelve:
    //     Un sufijo único que se anexa a la ruta de acceso de archivo. El valor predeterminado
    //     es "__ufps =" más un número único de 6 dígitos.
    protected internal virtual string UniqueFilePathSuffix
    {
        get
        {
            if (_uniqueFilePathSuffix != null)
            {
                return _uniqueFilePathSuffix;
            }

            long num = DateTime.Now.Ticks % 999983;
            _uniqueFilePathSuffix = string.Concat(UniqueFilePathSuffixID + "=", num.ToString("D6", CultureInfo.InvariantCulture));
            _uniqueFilePathSuffix = _uniqueFilePathSuffix.PadLeft(6, '0');
            return _uniqueFilePathSuffix;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el control en la página que se usa para realizar las devoluciones
    //     de datos.
    //
    // Devuelve:
    //     Control que se usa para realizar las devoluciones de datos.
    public Control AutoPostBackControl
    {
        get
        {
            return _autoPostBackControl;
        }
        set
        {
            _autoPostBackControl = value;
        }
    }

    internal bool ClientSupportsFocus
    {
        get
        {
            if (_request != null)
            {
                if (!(_request.Browser.EcmaScriptVersion >= FocusMinimumEcmaVersion))
                {
                    return _request.Browser.JScriptVersion >= FocusMinimumJScriptVersion;
                }

                return true;
            }

            return false;
        }
    }

    internal bool ClientSupportsJavaScript
    {
        get
        {
            if (!_clientSupportsJavaScriptChecked)
            {
                _clientSupportsJavaScript = _request != null && _request.Browser.EcmaScriptVersion >= JavascriptMinimumVersion;
                _clientSupportsJavaScriptChecked = true;
            }

            return _clientSupportsJavaScript;
        }
    }

    private ArrayList EnabledControls
    {
        get
        {
            if (_enabledControls == null)
            {
                _enabledControls = new ArrayList();
            }

            return _enabledControls;
        }
    }

    internal string FocusedControlID
    {
        get
        {
            if (_focusedControlID == null)
            {
                return string.Empty;
            }

            return _focusedControlID;
        }
    }

    internal Control FocusedControl => _focusedControl;

    //
    // Resumen:
    //     Obtiene el encabezado de documento de la página si la head se define el elemento
    //     con un runat=server en la declaración de la página.
    //
    // Devuelve:
    //     Un System.Web.UI.HtmlControls.HtmlHead que contiene el encabezado de página.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HtmlHead Header => _header;

    //
    // Resumen:
    //     Obtiene el carácter utilizado para separar los identificadores de control al
    //     construir un identificador único para un control en una página.
    //
    // Devuelve:
    //     El carácter utilizado para separar los identificadores de control. El valor predeterminado
    //     se establece el System.Web.UI.Adapters.PageAdapter instancia que representa la
    //     página. El System.Web.UI.Page.IdSeparator es un campo de servidor y no debe modificarse.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new virtual char IdSeparator
    {
        get
        {
            if (!_haveIdSeparator)
            {
                if (base.AdapterInternal != null)
                {
                    _idSeparator = PageAdapter.IdSeparator;
                }
                else
                {
                    _idSeparator = base.IdSeparatorFromConfig;
                }

                _haveIdSeparator = true;
            }

            return _idSeparator;
        }
    }

    internal string LastFocusedControl
    {
        [AspNetHostingPermission(SecurityAction.Assert, Level = AspNetHostingPermissionLevel.Low)]
        get
        {
            if (RequestInternal != null)
            {
                string text = Request["__LASTFOCUS"];
                if (text != null)
                {
                    return text;
                }
            }

            return string.Empty;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si el usuario regresará a la misma posición
    //     del explorador cliente después del postback. Esta propiedad reemplaza la propiedad
    //     obsoleta System.Web.UI.Page.SmartNavigation.
    //
    // Devuelve:
    //     Es true si se debe mantener la posición del cliente; de lo contrario, es false.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaintainScrollPositionOnPostBack
    {
        get
        {
            if (RequestInternal != null && RequestInternal.Browser != null && !RequestInternal.Browser.SupportsMaintainScrollPositionOnPostback)
            {
                return false;
            }

            return _maintainScrollPosition;
        }
        set
        {
            if (_maintainScrollPosition != value)
            {
                _maintainScrollPosition = value;
                if (_maintainScrollPosition)
                {
                    LoadScrollPosition();
                }
            }
        }
    }

    //
    // Resumen:
    //     Obtiene la página principal que determina la apariencia general de la página.
    //
    //
    // Devuelve:
    //     El System.Web.UI.MasterPage asociada a esta página si existe; de lo contrario,
    //     null.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [WebSysDescription("MasterPage_MasterPage")]
    public MasterPage Master
    {
        get
        {
            if (_master == null && !_preInitWorkComplete)
            {
                _master = MasterPage.CreateMaster(this, Context, _masterPageFile, _contentTemplateCollection);
            }

            return _master;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece la ruta de acceso virtual de la página maestra.
    //
    // Devuelve:
    //     La ruta de acceso virtual de la página maestra.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.MasterPageFile propiedad se establece después la System.Web.UI.Page.PreInit
    //     evento está completa.
    //
    //   T:System.Web.HttpException:
    //     El archivo especificado en el System.Web.UI.Page.MasterPageFile propiedad no
    //     existe. o bien La página no tiene un System.Web.UI.WebControls.Content como control
    //     de nivel superior.
    [DefaultValue("")]
    [WebCategory("Behavior")]
    [WebSysDescription("MasterPage_MasterPageFile")]
    public virtual string MasterPageFile
    {
        get
        {
            return VirtualPath.GetVirtualPathString(_masterPageFile);
        }
        set
        {
            if (_preInitWorkComplete)
            {
                throw new InvalidOperationException(SR.GetString("PropertySetBeforePageEvent", "MasterPageFile", "Page_PreInit"));
            }

            if (value != VirtualPath.GetVirtualPathString(_masterPageFile))
            {
                _masterPageFile = VirtualPath.CreateAllowNull(value);
                if (_master != null && Controls.Contains(_master))
                {
                    Controls.Remove(_master);
                }

                _master = null;
            }
        }
    }

    //
    // Resumen:
    //     Obtiene o establece la longitud máxima para el campo de estado de la página.
    //
    //
    // Devuelve:
    //     La longitud máxima, en bytes, para el campo de estado de la página. El valor
    //     predeterminado es -1.
    //
    // Excepciones:
    //   T:System.ArgumentException:
    //     El System.Web.UI.Page.MaxPageStateFieldLength propiedad no es igual a -1 o un
    //     número positivo.
    //
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.MaxPageStateFieldLength propiedad se estableció una vez
    //     inicializada la página.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int MaxPageStateFieldLength
    {
        get
        {
            return _maxPageStateFieldLength;
        }
        set
        {
            if (base.ControlState > ControlState.FrameworkInitialized)
            {
                throw new InvalidOperationException(SR.GetString("PropertySetAfterFrameworkInitialize", "MaxPageStateFieldLength"));
            }

            if (value == 0 || value < -1)
            {
                throw new ArgumentException(SR.GetString("Page_Illegal_MaxPageStateFieldLength"), "MaxPageStateFieldLength");
            }

            _maxPageStateFieldLength = value;
        }
    }

    internal bool ContainsCrossPagePost
    {
        get
        {
            return _containsCrossPagePost;
        }
        set
        {
            _containsCrossPagePost = value;
        }
    }

    internal bool RenderFocusScript => _requireFocusScript;

    internal Stack PartialCachingControlStack => _partialCachingControlStack;

    //
    // Resumen:
    //     Obtiene el System.Web.UI.PageStatePersister objeto asociado a la página.
    //
    // Devuelve:
    //     Un System.Web.UI.PageStatePersister asociada a la página.
    protected virtual PageStatePersister PageStatePersister
    {
        get
        {
            if (_persister == null)
            {
                PageAdapter pageAdapter = PageAdapter;
                if (pageAdapter != null)
                {
                    _persister = pageAdapter.GetStatePersister();
                }

                if (_persister == null)
                {
                    _persister = new HiddenFieldPageStatePersister(this);
                }
            }

            return _persister;
        }
    }

    internal string RequestViewStateString
    {
        get
        {
            if (!_cachedRequestViewState)
            {
                StringBuilder stringBuilder = new StringBuilder();
                try
                {
                    NameValueCollection requestValueCollection = RequestValueCollection;
                    if (requestValueCollection != null)
                    {
                        string text = RequestValueCollection["__VIEWSTATEFIELDCOUNT"];
                        if (MaxPageStateFieldLength == -1 || text == null)
                        {
                            _cachedRequestViewState = true;
                            _requestViewState = RequestValueCollection["__VIEWSTATE"];
                            return _requestViewState;
                        }

                        int num = Convert.ToInt32(text, CultureInfo.InvariantCulture);
                        if (num < 0)
                        {
                            throw new HttpException(SR.GetString("ViewState_InvalidViewState"));
                        }

                        for (int i = 0; i < num; i++)
                        {
                            string text2 = "__VIEWSTATE";
                            if (i > 0)
                            {
                                text2 += i.ToString(CultureInfo.InvariantCulture);
                            }

                            string text3 = RequestValueCollection[text2];
                            if (text3 == null)
                            {
                                throw new HttpException(SR.GetString("ViewState_MissingViewStateField", text2));
                            }

                            stringBuilder.Append(text3);
                        }
                    }

                    _cachedRequestViewState = true;
                    _requestViewState = stringBuilder.ToString();
                }
                catch (Exception inner)
                {
                    ViewStateException.ThrowViewStateError(inner, stringBuilder.ToString());
                }
            }

            return _requestViewState;
        }
    }

    internal string ValidatorInvalidControl
    {
        get
        {
            if (_validatorInvalidControl == null)
            {
                return string.Empty;
            }

            return _validatorInvalidControl;
        }
    }

    //
    // Resumen:
    //     Obtiene el System.Web.TraceContext objeto de la solicitud Web actual.
    //
    // Devuelve:
    //     Datos de la System.Web.TraceContext objeto de la solicitud Web actual.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TraceContext Trace => Context.Trace;

    //
    // Resumen:
    //     Obtiene el System.Web.HttpRequest objeto para la página solicitada.
    //
    // Devuelve:
    //     Actual System.Web.HttpRequest asociada a la página.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     Se produce cuando la System.Web.HttpRequest objeto no está disponible.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HttpRequest Request
    {
        get
        {
            if (_request == null)
            {
                throw new HttpException(SR.GetString("Request_not_available"));
            }

            return _request;
        }
    }

    internal HttpRequest RequestInternal => _request;

    //
    // Resumen:
    //     Obtiene el objeto System.Web.HttpResponse asociado al objeto System.Web.UI.Page.
    //     Este objeto permite enviar datos de respuesta HTTP a un cliente y contiene información
    //     sobre esa respuesta.
    //
    // Devuelve:
    //     Actual System.Web.HttpResponse asociada a la página.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     La System.Web.HttpResponse objeto no está disponible.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HttpResponse Response
    {
        get
        {
            if (_response == null)
            {
                throw new HttpException(SR.GetString("Response_not_available"));
            }

            return _response;
        }
    }

    //
    // Resumen:
    //     Obtiene el System.Web.Routing.RequestContext.RouteData valor del actual System.Web.Routing.RequestContext
    //     instancia.
    //
    // Devuelve:
    //     El System.Web.Routing.RequestContext.RouteData valor del actual System.Web.Routing.RequestContext
    //     instancia.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RouteData RouteData
    {
        get
        {
            if (Context != null && Context.Request != null)
            {
                return Context.Request.RequestContext.RouteData;
            }

            return null;
        }
    }

    //
    // Resumen:
    //     Obtiene el Server objeto, que es una instancia de la System.Web.HttpServerUtility
    //     clase.
    //
    // Devuelve:
    //     Actual Server objeto asociado a la página.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HttpServerUtility Server => Context.Server;

    //
    // Resumen:
    //     Obtiene el System.Web.Caching.Cache objeto asociado a la aplicación en el que
    //     reside la página.
    //
    // Devuelve:
    //     El System.Web.Caching.Cache asociado a la aplicación de la página.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     Una instancia de System.Web.Caching.Cache no se ha creado.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Cache Cache
    {
        get
        {
            if (_cache == null)
            {
                throw new HttpException(SR.GetString("Cache_not_available"));
            }

            return _cache;
        }
    }

    //
    // Resumen:
    //     Obtiene la actual Session objeto proporcionado por ASP.NET.
    //
    // Devuelve:
    //     Los datos de estado de la sesión actuales.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     Se produce cuando la información de sesión se establece en null.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual HttpSessionState Session
    {
        get
        {
            if (!_sessionRetrieved)
            {
                _sessionRetrieved = true;
                try
                {
                    _session = Context.Session;
                }
                catch
                {
                }
            }

            if (_session == null)
            {
                throw new HttpException(SR.GetString("Session_not_enabled"));
            }

            return _session;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el título de la página.
    //
    // Devuelve:
    //     El título de la página.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.Title propiedad requiere un control de encabezado en la
    //     página.
    [Bindable(true)]
    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Title
    {
        get
        {
            if (Page.Header == null && base.ControlState >= ControlState.ChildrenInitialized)
            {
                throw new InvalidOperationException(SR.GetString("Page_Title_Requires_Head"));
            }

            if (_titleToBeSet != null)
            {
                return _titleToBeSet;
            }

            return Page.Header.Title;
        }
        set
        {
            if (Page.Header == null)
            {
                if (base.ControlState >= ControlState.ChildrenInitialized)
                {
                    throw new InvalidOperationException(SR.GetString("Page_Title_Requires_Head"));
                }

                _titleToBeSet = value;
            }
            else
            {
                Page.Header.Title = value;
            }
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el contenido de la "Descripción" meta elemento.
    //
    // Devuelve:
    //     El contenido de la "Descripción" meta elemento.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     La página no tiene un control de encabezado (un head elemento con el runat atributo
    //     establecido en "server").
    [Bindable(true)]
    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MetaDescription
    {
        get
        {
            if (Page.Header == null && base.ControlState >= ControlState.ChildrenInitialized)
            {
                throw new InvalidOperationException(SR.GetString("Page_Description_Requires_Head"));
            }

            if (_descriptionToBeSet != null)
            {
                return _descriptionToBeSet;
            }

            return Page.Header.Description;
        }
        set
        {
            if (Page.Header == null)
            {
                if (base.ControlState >= ControlState.ChildrenInitialized)
                {
                    throw new InvalidOperationException(SR.GetString("Page_Description_Requires_Head"));
                }

                _descriptionToBeSet = value;
            }
            else
            {
                Page.Header.Description = value;
            }
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el contenido de "palabras clave" meta elemento.
    //
    // Devuelve:
    //     El contenido de las "palabras clave" meta elemento.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     La página no tiene un control de encabezado (un head elemento con el runat atributo
    //     establecido en "server").
    [Bindable(true)]
    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MetaKeywords
    {
        get
        {
            if (Page.Header == null && base.ControlState >= ControlState.ChildrenInitialized)
            {
                throw new InvalidOperationException(SR.GetString("Page_Keywords_Requires_Head"));
            }

            if (_keywordsToBeSet != null)
            {
                return _keywordsToBeSet;
            }

            return Page.Header.Keywords;
        }
        set
        {
            if (Page.Header == null)
            {
                if (base.ControlState >= ControlState.ChildrenInitialized)
                {
                    throw new InvalidOperationException(SR.GetString("Page_Keywords_Requires_Head"));
                }

                _keywordsToBeSet = value;
            }
            else
            {
                Page.Header.Keywords = value;
            }
        }
    }

    internal bool ContainsTheme => _theme != null;

    //
    // Resumen:
    //     Obtiene o establece el nombre del tema de la página.
    //
    // Devuelve:
    //     El nombre del tema de la página.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     Se intentó establecer System.Web.UI.Page.Theme después de la System.Web.UI.Page.PreInit
    //     se ha producido el evento.
    //
    //   T:System.ArgumentException:
    //     System.Web.UI.Page.Theme se establece en un nombre de tema no válido.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string Theme
    {
        get
        {
            return _themeName;
        }
        set
        {
            if (_preInitWorkComplete)
            {
                throw new InvalidOperationException(SR.GetString("PropertySetBeforePageEvent", "Theme", "Page_PreInit"));
            }

            if (!string.IsNullOrEmpty(value) && !FileUtil.IsValidDirectoryName(value))
            {
                throw new ArgumentException(SR.GetString("Page_theme_invalid_name", value), "Theme");
            }

            _themeName = value;
        }
    }

    internal bool SupportsStyleSheets
    {
        get
        {
            if (_supportsStyleSheets == -1)
            {
                if (Header != null && Header.StyleSheet != null && RequestInternal != null && Request.Browser != null && Request.Browser["preferredRenderingType"] != "xhtml-mp" && Request.Browser.SupportsCss && !Page.IsCallback && (ScriptManager == null || !ScriptManager.IsInAsyncPostBack))
                {
                    _supportsStyleSheets = 1;
                    return true;
                }

                _supportsStyleSheets = 0;
                return false;
            }

            return _supportsStyleSheets == 1;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el nombre del tema que se aplica a la página al principio
    //     en el ciclo de vida de la página.
    //
    // Devuelve:
    //     El nombre del tema que se aplica a la página al principio en el ciclo de vida
    //     de la página.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     Se ha intentado establecer el System.Web.UI.Page.StyleSheetTheme propiedad después
    //     de la System.Web.UI.Page.FrameworkInitialize se llamó al método.
    //
    //   T:System.ArgumentException:
    //     System.Web.UI.Page.StyleSheetTheme se establece en un nombre de tema no válido.
    //     Esta excepción se produce cuando el System.Web.UI.Page.FrameworkInitialize se
    //     llama el método, no por el establecedor de propiedad.
    [Browsable(false)]
    [Filterable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string StyleSheetTheme
    {
        get
        {
            return _styleSheetName;
        }
        set
        {
            if (_pageFlags[1])
            {
                throw new InvalidOperationException(SR.GetString("SetStyleSheetThemeCannotBeSet"));
            }

            _styleSheetName = value;
        }
    }

    //
    // Resumen:
    //     Obtiene información sobre el usuario que realiza la solicitud de página.
    //
    // Devuelve:
    //     Un System.Security.Principal.IPrincipal que representa al usuario que realiza
    //     la solicitud de página.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPrincipal User => Context.User;

    internal XhtmlConformanceMode XhtmlConformanceMode
    {
        get
        {
            if (!_xhtmlConformanceModeSet)
            {
                if (base.DesignMode)
                {
                    _xhtmlConformanceMode = XhtmlConformanceMode.Transitional;
                }
                else
                {
                    _xhtmlConformanceMode = GetXhtmlConformanceSection().Mode;
                }

                _xhtmlConformanceModeSet = true;
            }

            return _xhtmlConformanceMode;
        }
    }

    //
    // Resumen:
    //     Obtiene un valor que indica si la página participa en una devolución de datos
    //     entre páginas.
    //
    // Devuelve:
    //     true Si la página participa en una solicitud entre páginas; de lo contrario,
    //     false.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsCrossPagePostBack => _isCrossPagePostBack;

    internal bool IsExportingWebPart => _pageFlags[2];

    internal bool IsExportingWebPartShared => _pageFlags[4];

    //
    // Resumen:
    //     Obtiene un valor que indica si la página se representa por primera vez o se carga
    //     en respuesta a una devolución de datos.
    //
    // Devuelve:
    //     true Si la página se carga en respuesta a una devolución de datos del cliente;
    //     de lo contrario, false.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsPostBack
    {
        get
        {
            if (_requestValueCollection == null)
            {
                return false;
            }

            if (_isCrossPagePostBack)
            {
                return true;
            }

            if (_pageFlags[8])
            {
                return false;
            }

            if (ViewStateMacValidationErrorWasSuppressed)
            {
                return false;
            }

            if (Context.ServerExecuteDepth > 0 && (Context.Handler == null || GetType() != Context.Handler.GetType()))
            {
                return false;
            }

            return !_fPageLayoutChanged;
        }
    }

    internal NameValueCollection RequestValueCollection => _requestValueCollection;

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si la página valida los eventos de devolución
    //     y devolución de llamada.
    //
    // Devuelve:
    //     true Si la página valida los eventos de devolución y devolución de llamada; de
    //     lo contrario, false. El valor predeterminado es true.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.EnableEventValidation propiedad se estableció una vez inicializada
    //     la página.
    [Browsable(false)]
    [DefaultValue(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool EnableEventValidation
    {
        get
        {
            return _enableEventValidation;
        }
        set
        {
            if (base.ControlState > ControlState.FrameworkInitialized)
            {
                throw new InvalidOperationException(SR.GetString("PropertySetAfterFrameworkInitialize", "EnableEventValidation"));
            }

            _enableEventValidation = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si la página mantiene su estado de vista
    //     y controla el estado de vista de cualquier servidor contiene, cuando finaliza
    //     la solicitud de página actual.
    //
    // Devuelve:
    //     true Si la página mantiene su estado de vista; de lo contrario, false. De manera
    //     predeterminada, es true.
    [Browsable(false)]
    public override bool EnableViewState
    {
        get
        {
            return base.EnableViewState;
        }
        set
        {
            base.EnableViewState = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece el modo de cifrado del estado de vista.
    //
    // Devuelve:
    //     Uno de los valores de System.Web.UI.ViewStateEncryptionMode. El valor predeterminado
    //     es System.Web.UI.ViewStateEncryptionMode.Auto.
    //
    // Excepciones:
    //   T:System.ArgumentOutOfRangeException:
    //     El valor establecido no es un miembro de la System.Web.UI.ViewStateEncryptionMode
    //     (enumeración).
    //
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.ViewStateEncryptionMode propiedad puede establecerse únicamente
    //     en o antes de la página PreRenderfase en el ciclo de vida de la página.
    [Browsable(false)]
    [DefaultValue(ViewStateEncryptionMode.Auto)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ViewStateEncryptionMode ViewStateEncryptionMode
    {
        get
        {
            return _encryptionMode;
        }
        set
        {
            if (base.ControlState > ControlState.FrameworkInitialized)
            {
                throw new InvalidOperationException(SR.GetString("PropertySetAfterFrameworkInitialize", "ViewStateEncryptionMode"));
            }

            if (value < ViewStateEncryptionMode.Auto || value > ViewStateEncryptionMode.Never)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            _encryptionMode = value;
        }
    }

    //
    // Resumen:
    //     Asigna un identificador a un usuario individual en la variable de estado de vista
    //     asociado a la página actual.
    //
    // Devuelve:
    //     El identificador de usuario individual.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     El System.Web.UI.Page.ViewStateUserKey propiedad accedió demasiado tarde durante
    //     el procesamiento de la página.
    [Browsable(false)]
    public string ViewStateUserKey
    {
        get
        {
            return _viewStateUserKey;
        }
        set
        {
            if (base.ControlState >= ControlState.Initialized)
            {
                throw new HttpException(SR.GetString("Too_late_for_ViewStateUserKey"));
            }

            _viewStateUserKey = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un identificador para una instancia determinada de la System.Web.UI.Page
    //     clase.
    //
    // Devuelve:
    //     El identificador de la instancia de la System.Web.UI.Page clase. El valor predeterminado
    //     es '_Page'.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ID
    {
        get
        {
            return base.ID;
        }
        set
        {
            base.ID = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si la página comprueba la entrada del
    //     cliente desde el Explorador de valores potencialmente peligrosos.
    //
    // Devuelve:
    //     Un valor que indica si la página comprueba la entrada del cliente. De manera
    //     predeterminada, es System.Web.UI.ValidateRequestMode.Enabled.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(ValidateRequestMode.Enabled)]
    public override ValidateRequestMode ValidateRequestMode
    {
        get
        {
            return base.ValidateRequestMode;
        }
        set
        {
            base.ValidateRequestMode = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si se valida el valor de cadena de consulta.
    //
    //
    // Devuelve:
    //     true Si se debe omitir la validación de la cadena de consulta (no se debe validar
    //     la cadena de consulta); de lo contrario, false colocar como normal si debería
    //     tener la validación de cadena de consulta. De manera predeterminada, es false.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(false)]
    public bool SkipFormActionValidation
    {
        get
        {
            return _pageFlags[64];
        }
        set
        {
            if (value != SkipFormActionValidation)
            {
                _clientQueryString = null;
            }

            _pageFlags[64] = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si la System.Web.UI.Page representa el
    //     objeto.
    //
    // Devuelve:
    //     true Si la System.Web.UI.Page es representado; en caso contrario, false. De manera
    //     predeterminada, es true.
    [Browsable(false)]
    public override bool Visible
    {
        get
        {
            return base.Visible;
        }
        set
        {
            base.Visible = value;
        }
    }

    private bool ViewStateMacValidationErrorWasSuppressed
    {
        get
        {
            return _pageFlags[128];
        }
        set
        {
            _pageFlags[128] = value;
        }
    }

    private bool RenderDisabledControlsScript
    {
        get
        {
            if (Form.SubmitDisabledControls && EnabledControls.Count > 0)
            {
                return _request.Browser.W3CDomVersion.Major > 0;
            }

            return false;
        }
    }

    internal bool IsInOnFormRender => _inOnFormRender;

    //
    // Resumen:
    //     Obtiene un valor que indica si se ha registrado el control en la página que realiza
    //     devoluciones de datos.
    //
    // Devuelve:
    //     true Si el control tiene ha registrado; de lo contrario, false.
    public bool IsPostBackEventControlRegistered => _registeredControlThatRequireRaiseEvent != null;

    //
    // Resumen:
    //     Obtiene un valor que indica si la validación de la página se realizó correctamente.
    //
    //
    // Devuelve:
    //     true Si se realizó correctamente la validación de la página; de lo contrario,
    //     false.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     El System.Web.UI.Page.IsValid propiedad se llama antes de que se ha producido
    //     validación.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsValid
    {
        get
        {
            if (!_validated)
            {
                throw new HttpException(SR.GetString("IsValid_Cant_Be_Called"));
            }

            if (_validators != null)
            {
                ValidatorCollection validators = Validators;
                int count = validators.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!validators[i].IsValid)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    //
    // Resumen:
    //     Obtiene una colección de todos los controles de validación contenidos en la página
    //     solicitada.
    //
    // Devuelve:
    //     La colección de controles de validación.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ValidatorCollection Validators
    {
        get
        {
            if (_validators == null)
            {
                _validators = new ValidatorCollection();
            }

            return _validators;
        }
    }

    //
    // Resumen:
    //     Obtiene la página que transfirió el control a la página actual.
    //
    // Devuelve:
    //     El System.Web.UI.Page que representa la página que transfirió el control a la
    //     página actual.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El usuario actual no tiene acceso a la página anterior. O bien Enrutamiento de
    //     ASP.NET está en uso y la dirección URL de la página anterior es una dirección
    //     URL enrutada. Cuando ASP.NET comprueba los permisos de acceso, se supone que
    //     la dirección URL es una ruta de acceso real a un archivo. Dado que no es el caso
    //     con una dirección URL enrutada, se produce un error en la comprobación.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Page PreviousPage
    {
        get
        {
            if (_previousPage == null && _previousPagePath != null)
            {
                if (!Util.IsUserAllowedToPath(Context, _previousPagePath))
                {
                    throw new InvalidOperationException(SR.GetString("Previous_Page_Not_Authorized"));
                }

                ITypedWebObjectFactory typedWebObjectFactory = (ITypedWebObjectFactory)BuildManager.GetVPathBuildResult(Context, _previousPagePath);
                if (typeof(Page).IsAssignableFrom(typedWebObjectFactory.InstantiatedType))
                {
                    _previousPage = (Page)typedWebObjectFactory.CreateInstance();
                    _previousPage._isCrossPagePostBack = true;
                    Server.Execute(_previousPage, TextWriter.Null, preserveForm: true, setPreviousPage: false);
                }
            }

            return _previousPage;
        }
    }

    //
    // Resumen:
    //     Establece una matriz de archivos que actual System.Web.HttpResponse objeto depende.
    //
    //
    // Devuelve:
    //     La matriz de archivos actual System.Web.HttpResponse objeto depende.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("The recommended alternative is HttpResponse.AddFileDependencies. http://go.microsoft.com/fwlink/?linkid=14202")]
    protected ArrayList FileDependencies
    {
        set
        {
            Response.AddFileDependencies(value);
        }
    }

    //
    // Resumen:
    //     Establece un valor que indica si el resultado de la página se almacena en búfer.
    //
    //
    // Devuelve:
    //     true Si se almacena en búfer el resultado de la página; de lo contrario, false.
    //     De manera predeterminada, es true.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Buffer
    {
        get
        {
            return Response.BufferOutput;
        }
        set
        {
            Response.BufferOutput = value;
        }
    }

    //
    // Resumen:
    //     Establece el tipo MIME HTTP de la System.Web.HttpResponse objeto asociado a la
    //     página.
    //
    // Devuelve:
    //     El tipo MIME HTTP asociado a la página actual.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ContentType
    {
        get
        {
            return Response.ContentType;
        }
        set
        {
            Response.ContentType = value;
        }
    }

    //
    // Resumen:
    //     Establece el identificador de página de códigos actual System.Web.UI.Page.
    //
    // Devuelve:
    //     Un entero que representa el identificador de página de códigos actual System.Web.UI.Page.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CodePage
    {
        get
        {
            return Response.ContentEncoding.CodePage;
        }
        set
        {
            Response.ContentEncoding = Encoding.GetEncoding(value);
        }
    }

    //
    // Resumen:
    //     Establece el lenguaje de codificación actual System.Web.HttpResponse objeto.
    //
    //
    // Devuelve:
    //     Una cadena que contiene el lenguaje de codificación actual System.Web.HttpResponse.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ResponseEncoding
    {
        get
        {
            return Response.ContentEncoding.EncodingName;
        }
        set
        {
            Response.ContentEncoding = Encoding.GetEncoding(value);
        }
    }

    //
    // Resumen:
    //     Establece el identificador de referencia cultural para la System.Threading.Thread
    //     objeto asociado a la página.
    //
    // Devuelve:
    //     Un identificador de referencia cultural válida.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Culture
    {
        get
        {
            return Thread.CurrentThread.CurrentCulture.DisplayName;
        }
        set
        {
            CultureInfo cultureInfo = null;
            if (StringUtil.EqualsIgnoreCase(value, HttpApplication.AutoCulture))
            {
                CultureInfo cultureInfo2 = CultureFromUserLanguages(specific: true);
                if (cultureInfo2 != null)
                {
                    cultureInfo = cultureInfo2;
                }
            }
            else if (StringUtil.StringStartsWithIgnoreCase(value, HttpApplication.AutoCulture))
            {
                CultureInfo cultureInfo3 = CultureFromUserLanguages(specific: true);
                if (cultureInfo3 != null)
                {
                    cultureInfo = cultureInfo3;
                }
                else
                {
                    try
                    {
                        cultureInfo = HttpServerUtility.CreateReadOnlyCultureInfo(value.Substring(5));
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                cultureInfo = HttpServerUtility.CreateReadOnlyCultureInfo(value);
            }

            if (cultureInfo != null)
            {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                _dynamicCulture = cultureInfo;
            }
        }
    }

    internal CultureInfo DynamicCulture => _dynamicCulture;

    //
    // Resumen:
    //     Establece el identificador de configuración regional para el System.Threading.Thread
    //     objeto asociado a la página.
    //
    // Devuelve:
    //     El identificador de configuración regional para pasar a la System.Threading.Thread.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LCID
    {
        get
        {
            return Thread.CurrentThread.CurrentCulture.LCID;
        }
        set
        {
            CultureInfo cultureInfo = HttpServerUtility.CreateReadOnlyCultureInfo(value);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            _dynamicCulture = cultureInfo;
        }
    }

    //
    // Resumen:
    //     Establece el identificador de interfaz de usuario para la System.Threading.Thread
    //     objeto asociado a la página.
    //
    // Devuelve:
    //     El identificador de interfaz de usuario.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UICulture
    {
        get
        {
            return Thread.CurrentThread.CurrentUICulture.DisplayName;
        }
        set
        {
            CultureInfo cultureInfo = null;
            if (StringUtil.EqualsIgnoreCase(value, HttpApplication.AutoCulture))
            {
                CultureInfo cultureInfo2 = CultureFromUserLanguages(specific: false);
                if (cultureInfo2 != null)
                {
                    cultureInfo = cultureInfo2;
                }
            }
            else if (StringUtil.StringStartsWithIgnoreCase(value, HttpApplication.AutoCulture))
            {
                CultureInfo cultureInfo3 = CultureFromUserLanguages(specific: false);
                if (cultureInfo3 != null)
                {
                    cultureInfo = cultureInfo3;
                }
                else
                {
                    try
                    {
                        cultureInfo = HttpServerUtility.CreateReadOnlyCultureInfo(value.Substring(5));
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                cultureInfo = HttpServerUtility.CreateReadOnlyCultureInfo(value);
            }

            if (cultureInfo != null)
            {
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                _dynamicUICulture = cultureInfo;
            }
        }
    }

    internal CultureInfo DynamicUICulture => _dynamicUICulture;

    //
    // Resumen:
    //     Obtiene o establece un valor que indica el intervalo de tiempo de espera utilizado
    //     al procesar tareas asincrónicas.
    //
    // Devuelve:
    //     Un System.TimeSpan que contiene el intervalo de tiempo permitido para la finalización
    //     de la tarea asincrónica. El intervalo de tiempo predeterminado es 45 segundos.
    //
    //
    // Excepciones:
    //   T:System.ArgumentException:
    //     La propiedad se estableció en un valor negativo.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan AsyncTimeout
    {
        get
        {
            if (!_asyncTimeoutSet)
            {
                if (Context != null)
                {
                    PagesSection pages = RuntimeConfig.GetConfig(Context).Pages;
                    if (pages != null)
                    {
                        AsyncTimeout = pages.AsyncTimeout;
                    }
                }

                if (!_asyncTimeoutSet)
                {
                    AsyncTimeout = TimeSpan.FromSeconds(DefaultAsyncTimeoutSeconds);
                }
            }

            return _asyncTimeout;
        }
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentException(SR.GetString("Page_Illegal_AsyncTimeout"), "AsyncTimeout");
            }

            _asyncTimeout = value;
            _asyncTimeoutSet = true;
        }
    }

    //
    // Resumen:
    //     Establece el nivel de transacción capacidad de la página.
    //
    // Devuelve:
    //     Un entero que representa uno de los System.EnterpriseServices.TransactionOption
    //     miembros de la enumeración.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int TransactionMode
    {
        get
        {
            return _transactionMode;
        }
        set
        {
            _transactionMode = value;
        }
    }

    //
    // Resumen:
    //     Establece un valor que indica si la página se puede ejecutar en un subproceso
    //     en un contenedor uniproceso (STA).
    //
    // Devuelve:
    //     true Si la página admite el código de páginas Active Server (ASP); de lo contrario,
    //     false. De manera predeterminada, es false.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool AspCompatMode
    {
        get
        {
            return _aspCompatMode;
        }
        set
        {
            _aspCompatMode = value;
        }
    }

    //
    // Resumen:
    //     Establece un valor que indica si la página se procesa de forma sincrónica o asincrónica.
    //
    //
    // Devuelve:
    //     true Si la página se procesa de forma asincrónica; de lo contrario, false.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool AsyncMode
    {
        get
        {
            return _asyncMode;
        }
        set
        {
            _asyncMode = value;
        }
    }

    //
    // Resumen:
    //     Establece un valor que indica si el seguimiento está habilitado para la System.Web.UI.Page
    //     objeto.
    //
    // Devuelve:
    //     true Si el seguimiento está habilitado para la página. de lo contrario, false.
    //     De manera predeterminada, es false.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool TraceEnabled
    {
        get
        {
            return Trace.IsEnabled;
        }
        set
        {
            Trace.IsEnabled = value;
        }
    }

    //
    // Resumen:
    //     Establece el modo de seguimiento que se muestran las instrucciones en la página.
    //
    //
    // Devuelve:
    //     Uno de los System.Web.TraceMode miembros de la enumeración.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TraceMode TraceModeValue
    {
        get
        {
            return Trace.TraceMode;
        }
        set
        {
            Trace.TraceMode = value;
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si ASP.NET debe comprobar los códigos
    //     de autenticación de mensajes (MAC) en el estado de vista cuando la página se
    //     devuelve desde el cliente.
    //
    // Devuelve:
    //     true Si el estado de vista debe ser MAC comprueba y codificado; de lo contrario,
    //     false. De manera predeterminada, es true.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableViewStateMac
    {
        get
        {
            return _enableViewStateMac;
        }
        set
        {
            if (!EnableViewStateMacRegistryHelper.EnforceViewStateMac)
            {
                _enableViewStateMac = value;
            }
        }
    }

    //
    // Resumen:
    //     Obtiene o establece un valor que indica si está habilitada la navegación inteligente.
    //     Esta propiedad está desusada.
    //
    // Devuelve:
    //     Es true si la animación inteligente está habilitada; de lo contrario, es false.
    [Browsable(false)]
    [Filterable(false)]
    [Obsolete("The recommended alternative is Page.SetFocus and Page.MaintainScrollPositionOnPostBack. http://go.microsoft.com/fwlink/?linkid=14202")]
    public bool SmartNavigation
    {
        get
        {
            if (_smartNavSupport == SmartNavigationSupport.NotDesiredOrSupported)
            {
                return false;
            }

            if (_smartNavSupport == SmartNavigationSupport.Desired)
            {
                HttpContext current = HttpContext.Current;
                if (current == null)
                {
                    return false;
                }

                HttpBrowserCapabilities browser = current.Request.Browser;
                if (!string.Equals(browser.Browser, "ie", StringComparison.OrdinalIgnoreCase) || browser.MajorVersion < 6 || !browser.Win32)
                {
                    _smartNavSupport = SmartNavigationSupport.NotDesiredOrSupported;
                }
                else
                {
                    _smartNavSupport = SmartNavigationSupport.IE6OrNewer;
                }
            }

            return _smartNavSupport != SmartNavigationSupport.NotDesiredOrSupported;
        }
        set
        {
            if (value)
            {
                _smartNavSupport = SmartNavigationSupport.Desired;
            }
            else
            {
                _smartNavSupport = SmartNavigationSupport.NotDesiredOrSupported;
            }
        }
    }

    internal bool IsTransacted => _transactionMode != 0;

    internal bool IsInAspCompatMode => _aspCompatMode;

    //
    // Resumen:
    //     Obtiene un valor que indica si la página se procesa de forma asincrónica.
    //
    // Devuelve:
    //     true Si la página está en modo asincrónico; de lo contrario, false;
    public bool IsAsync => _asyncMode;

    internal bool RequiresViewStateEncryptionInternal
    {
        get
        {
            if (ViewStateEncryptionMode != ViewStateEncryptionMode.Always)
            {
                if (_viewStateEncryptionRequested)
                {
                    return ViewStateEncryptionMode == ViewStateEncryptionMode.Auto;
                }

                return false;
            }

            return true;
        }
    }

    //
    // Resumen:
    //     Obtiene el formulario HTML de la página.
    //
    // Devuelve:
    //     La System.Web.UI.HtmlControls.HtmlForm objeto asociado a la página.
    public HtmlForm Form => _form;

    //
    // Resumen:
    //     Obtiene el adaptador que representa la página del explorador solicitante específicos.
    //
    //
    // Devuelve:
    //     El System.Web.UI.Adapters.PageAdapter que representa la página.
    public PageAdapter PageAdapter
    {
        get
        {
            if (_pageAdapter == null)
            {
                ResolveAdapter();
                _pageAdapter = (PageAdapter)base.AdapterInternal;
            }

            return _pageAdapter;
        }
    }

    internal string RelativeFilePath
    {
        get
        {
            if (_relativeFilePath == null)
            {
                string text = Context.Request.CurrentExecutionFilePath;
                string filePath = Context.Request.FilePath;
                if (filePath.Equals(text))
                {
                    int num = text.LastIndexOf('/');
                    if (num >= 0)
                    {
                        text = text.Substring(num + 1);
                    }

                    _relativeFilePath = text;
                }
                else
                {
                    _relativeFilePath = Server.UrlDecode(UrlPath.MakeRelative(filePath, text));
                }
            }

            return _relativeFilePath;
        }
    }

    //
    // Resumen:
    //     Obtiene una lista de objetos almacenados en el contexto de la página.
    //
    // Devuelve:
    //     Una referencia a un System.Collections.IDictionary que contiene los objetos almacenados
    //     en el contexto de la página.
    [Browsable(false)]
    public IDictionary Items
    {
        get
        {
            if (_items == null)
            {
                _items = new HybridDictionary();
            }

            return _items;
        }
    }

    internal IScriptManager ScriptManager => (IScriptManager)Items[typeof(IScriptManager)];

    internal bool IsPartialRenderingSupported
    {
        get
        {
            if (!_pageFlags[32])
            {
                Type scriptManagerType = ScriptManagerType;
                if (scriptManagerType != null)
                {
                    object obj = Page.Items[scriptManagerType];
                    if (obj != null)
                    {
                        PropertyInfo property = scriptManagerType.GetProperty("SupportsPartialRendering");
                        if (property != null)
                        {
                            object value = property.GetValue(obj, null);
                            _pageFlags[16] = (bool)value;
                        }
                    }
                }

                _pageFlags[32] = true;
            }

            return _pageFlags[16];
        }
    }

    internal Type ScriptManagerType
    {
        get
        {
            if (_scriptManagerType == null)
            {
                _scriptManagerType = BuildManager.GetType("System.Web.UI.ScriptManager", throwOnError: false);
            }

            return _scriptManagerType;
        }
        set
        {
            _scriptManagerType = value;
        }
    }

    //
    // Resumen:
    //     Se produce al final de la fase de carga del ciclo de vida de la página.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler LoadComplete
    {
        add
        {
            base.Events.AddHandler(EventLoadComplete, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventLoadComplete, value);
        }
    }

    //
    // Resumen:
    //     Se produce antes de la inicialización de la página.
    public event EventHandler PreInit
    {
        add
        {
            base.Events.AddHandler(EventPreInit, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventPreInit, value);
        }
    }

    //
    // Resumen:
    //     Se produce antes de la página System.Web.UI.Control.Load eventos.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler PreLoad
    {
        add
        {
            base.Events.AddHandler(EventPreLoad, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventPreLoad, value);
        }
    }

    //
    // Resumen:
    //     Se produce antes de que se represente el contenido de la página.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler PreRenderComplete
    {
        add
        {
            base.Events.AddHandler(EventPreRenderComplete, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventPreRenderComplete, value);
        }
    }

    //
    // Resumen:
    //     Se produce cuando se completa la inicialización de la página.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler InitComplete
    {
        add
        {
            base.Events.AddHandler(EventInitComplete, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventInitComplete, value);
        }
    }

    //
    // Resumen:
    //     Se produce después de la página ha terminado de guardar toda la vista estado
    //     y control de información de estado de la página y controles de la página.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler SaveStateComplete
    {
        add
        {
            base.Events.AddHandler(EventSaveStateComplete, value);
        }
        remove
        {
            base.Events.RemoveHandler(EventSaveStateComplete, value);
        }
    }

    static Page()
    {
        EventPreRenderComplete = new object();
        EventPreLoad = new object();
        EventLoadComplete = new object();
        EventPreInit = new object();
        EventInitComplete = new object();
        EventSaveStateComplete = new object();
        FocusMinimumEcmaVersion = new Version("1.4");
        FocusMinimumJScriptVersion = new Version("3.0");
        JavascriptMinimumVersion = new Version("1.0");
        MSDomScrollMinimumVersion = new Version("4.0");
        UniqueFilePathSuffixID = "__ufps";
        DefaultMaxPageStateFieldLength = -1;
        DefaultAsyncTimeoutSeconds = 45;
        _maxAsyncTimeout = TimeSpan.FromMilliseconds(2147483647.0);
        s_varySeparator = new char[1] { ';' };
        s_systemPostFields = new StringSet();
        s_systemPostFields.Add("__EVENTTARGET");
        s_systemPostFields.Add("__EVENTARGUMENT");
        s_systemPostFields.Add("__VIEWSTATEFIELDCOUNT");
        s_systemPostFields.Add("__VIEWSTATEGENERATOR");
        s_systemPostFields.Add("__VIEWSTATE");
        s_systemPostFields.Add("__VIEWSTATEENCRYPTED");
        s_systemPostFields.Add("__PREVIOUSPAGE");
        s_systemPostFields.Add("__CALLBACKID");
        s_systemPostFields.Add("__CALLBACKPARAM");
        s_systemPostFields.Add("__LASTFOCUS");
        s_systemPostFields.Add(UniqueFilePathSuffixID);
        s_systemPostFields.Add(HttpResponse.RedirectQueryStringVariable);
        s_systemPostFields.Add("__EVENTVALIDATION");
    }

    //
    // Resumen:
    //     Inicializa una nueva instancia de la clase System.Web.UI.Page.
    public Page()
    {
        _page = this;
        _enableViewStateMac = true;
        ID = "__Page";
        _supportsStyleSheets = -1;
        SetValidateRequestModeInternal(ValidateRequestMode.Enabled, setDirty: false);
    }

    internal void SetActiveValueProvider(IValueProvider valueProvider)
    {
        ActiveValueProvider = valueProvider;
    }

    //
    // Resumen:
    //     Actualiza la instancia de modelo especificada usando valores del control enlazado
    //     a datos.
    //
    // Parámetros:
    //   model:
    //     Modelo.
    //
    // Parámetros de tipo:
    //   TModel:
    //     El tipo del modelo.
    //
    // Devuelve:
    //     true Si el enlace de modelos se realiza correctamente; de lo contrario, false.
    public virtual bool TryUpdateModel<TModel>(TModel model) where TModel : class
    {
        if (ActiveValueProvider == null)
        {
            throw new InvalidOperationException(SR.GetString("Page_InvalidUpdateModelAttempt", "TryUpdateModel"));
        }

        return TryUpdateModel(model, ActiveValueProvider);
    }

    //
    // Resumen:
    //     Actualiza la instancia de modelo usando valores del proveedor de valor especificado.
    //
    //
    // Parámetros:
    //   model:
    //     Modelo.
    //
    //   valueProvider:
    //     Proveedor de valores.
    //
    // Parámetros de tipo:
    //   TModel:
    //     El tipo del modelo.
    //
    // Devuelve:
    //     true Si el enlace de modelos se realiza correctamente; de lo contrario, false.
    public virtual bool TryUpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
    {
        if (model == null)
        {
            throw new ArgumentNullException("model");
        }

        if (valueProvider == null)
        {
            throw new ArgumentNullException("valueProvider");
        }

        IModelBinder defaultBinder = ModelBinders.Binders.DefaultBinder;
        ModelBindingContext modelBindingContext = new ModelBindingContext();
        modelBindingContext.ModelBinderProviders = ModelBinderProviders.Providers;
        modelBindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel));
        modelBindingContext.ModelState = ModelState;
        modelBindingContext.ValueProvider = valueProvider;
        ModelBindingContext bindingContext = modelBindingContext;
        if (defaultBinder.BindModel(ModelBindingExecutionContext, bindingContext))
        {
            return ModelState.IsValid;
        }

        return false;
    }

    //
    // Resumen:
    //     Actualiza la instancia de modelo especificada usando valores del control enlazado
    //     a datos.
    //
    // Parámetros:
    //   model:
    //     Modelo.
    //
    // Parámetros de tipo:
    //   TModel:
    //     El tipo del modelo.
    public virtual void UpdateModel<TModel>(TModel model) where TModel : class
    {
        if (ActiveValueProvider == null)
        {
            throw new InvalidOperationException(SR.GetString("Page_InvalidUpdateModelAttempt", "UpdateModel"));
        }

        UpdateModel(model, ActiveValueProvider);
    }

    //
    // Resumen:
    //     Actualiza la instancia de modelo especificada usando valores del proveedor de
    //     valor especificado.
    //
    // Parámetros:
    //   model:
    //     Modelo.
    //
    //   valueProvider:
    //     Proveedor de valores.
    //
    // Parámetros de tipo:
    //   TModel:
    //     El tipo del modelo.
    public virtual void UpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
    {
        if (!TryUpdateModel(model, valueProvider))
        {
            throw new InvalidOperationException(SR.GetString("Page_UpdateModel_UpdateUnsuccessful", typeof(TModel).FullName));
        }
    }

    //
    // Resumen:
    //     Crea un System.Web.UI.HtmlTextWriter objeto para representar el contenido de
    //     la página.
    //
    // Parámetros:
    //   tw:
    //     El objeto System.IO.TextWriter utilizado para crear el objeto System.Web.UI.HtmlTextWriter.
    //
    //
    // Devuelve:
    //     Un System.Web.UI.HtmlTextWriter o System.Web.UI.Html32TextWriter.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual HtmlTextWriter CreateHtmlTextWriter(TextWriter tw)
    {
        if (Context != null && Context.Request != null && Context.Request.Browser != null)
        {
            return Context.Request.Browser.CreateHtmlTextWriter(tw);
        }

        HtmlTextWriter htmlTextWriter = CreateHtmlTextWriterInternal(tw, _request);
        if (htmlTextWriter == null)
        {
            htmlTextWriter = new HtmlTextWriter(tw);
        }

        return htmlTextWriter;
    }

    internal static HtmlTextWriter CreateHtmlTextWriterInternal(TextWriter tw, HttpRequest request)
    {
        if (request != null && request.Browser != null)
        {
            return request.Browser.CreateHtmlTextWriterInternal(tw);
        }

        return new Html32TextWriter(tw);
    }

    //
    // Resumen:
    //     Crea un objeto System.Web.UI.HtmlTextWriter objeto para representar el contenido
    //     de la página.
    //
    // Parámetros:
    //   tw:
    //     El objeto System.IO.TextWriter utilizado para crear el objeto System.Web.UI.HtmlTextWriter.
    //
    //
    //   writerType:
    //     El tipo de escritor de texto para crear.
    //
    // Devuelve:
    //     Un System.Web.UI.HtmlTextWriter que representa el contenido de la página.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     El writerType parámetro está establecido en un tipo no válido.
    public static HtmlTextWriter CreateHtmlTextWriterFromType(TextWriter tw, Type writerType)
    {
        if (writerType == typeof(HtmlTextWriter))
        {
            return new HtmlTextWriter(tw);
        }

        if (writerType == typeof(Html32TextWriter))
        {
            return new Html32TextWriter(tw);
        }

        try
        {
            Util.CheckAssignableType(typeof(HtmlTextWriter), writerType);
            return (HtmlTextWriter)HttpRuntime.CreateNonPublicInstance(writerType, new object[1] { tw });
        }
        catch
        {
            throw new HttpException(SR.GetString("Invalid_HtmlTextWriter", writerType.FullName));
        }
    }

    //
    // Resumen:
    //     Busca un control de servidor con el identificador especificado en el contenedor
    //     de nomenclatura de la página.
    //
    // Parámetros:
    //   id:
    //     El identificador del control que se encuentra.
    //
    // Devuelve:
    //     El control especificado, o null Si el control especificado no existe.
    public override Control FindControl(string id)
    {
        if (StringUtil.EqualsIgnoreCase(id, "__Page"))
        {
            return this;
        }

        return base.FindControl(id, 0);
    }

    //
    // Resumen:
    //     Recupera el código hash generado por System.Web.UI.Page objetos que se generan
    //     en tiempo de ejecución. Este código hash es único en el System.Web.UI.Page jerarquía
    //     de control del objeto.
    //
    // Devuelve:
    //     Código hash generado en tiempo de ejecución. El valor predeterminado es 0.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual int GetTypeHashCode()
    {
        return 0;
    }

    internal override string GetUniqueIDPrefix()
    {
        if (Parent == null)
        {
            return string.Empty;
        }

        return base.GetUniqueIDPrefix();
    }

    internal uint GetClientStateIdentifier()
    {
        int nonRandomizedHashCode = StringUtil.GetNonRandomizedHashCode(TemplateSourceDirectory, ignoreCase: true);
        return (uint)(nonRandomizedHashCode + StringUtil.GetNonRandomizedHashCode(GetType().Name, ignoreCase: true));
    }

    private bool HandleError(Exception e)
    {
        try
        {
            Context.TempError = e;
            OnError(EventArgs.Empty);
            if (Context.TempError == null)
            {
                return true;
            }
        }
        finally
        {
            Context.TempError = null;
        }

        if (!string.IsNullOrEmpty(_errorPage) && Context.IsCustomErrorEnabled)
        {
            _response.RedirectToErrorPage(_errorPage, CustomErrorsSection.GetSettings(Context).RedirectMode);
            return true;
        }

        PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_UNHANDLED);
        string postMessage = null;
        if (Context.TraceIsEnabled)
        {
            Trace.Warn(SR.GetString("Unhandled_Err_Error"), null, e);
            if (Trace.PageOutput)
            {
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter output = new HtmlTextWriter(stringWriter);
                BuildPageProfileTree(enableViewState: false);
                Trace.EndRequest();
                Trace.StopTracing();
                Trace.StatusCode = 500;
                Trace.Render(output);
                postMessage = stringWriter.ToString();
            }
        }

        if (HttpException.GetErrorFormatter(e) != null)
        {
            return false;
        }

        if (e is SecurityException)
        {
            return false;
        }

        throw new HttpUnhandledException(null, postMessage, e);
    }

    internal static string DecryptString(string s, Purpose purpose)
    {
        if (s == null)
        {
            return null;
        }

        byte[] array = HttpServerUtility.UrlTokenDecode(s);
        byte[] array2 = null;
        if (array != null)
        {
            if (AspNetCryptoServiceProvider.Instance.IsDefaultProvider)
            {
                ICryptoService cryptoService = AspNetCryptoServiceProvider.Instance.GetCryptoService(purpose, CryptoServiceOptions.CacheableOutput);
                array2 = cryptoService.Unprotect(array);
            }
            else
            {
                array2 = MachineKeySection.EncryptOrDecryptData(fEncrypt: false, array, null, 0, array.Length, useValidationSymAlgo: false, useLegacyMode: false, IVType.Hash);
            }
        }

        if (array2 == null)
        {
            throw new HttpException(SR.GetString("ViewState_InvalidViewState"));
        }

        return Encoding.UTF8.GetString(array2);
    }

    //
    // Resumen:
    //     Realiza las inicializaciones de la instancia de la System.Web.UI.Page clase que
    //     requieran los diseñadores RAD. Este método se usa sólo en tiempo de diseño.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void DesignerInitialize()
    {
        InitRecursive(null);
    }

    internal NameValueCollection GetCollectionBasedOnMethod(bool dontReturnNull)
    {
        if (_request.HttpVerb == HttpVerb.POST)
        {
            if (!dontReturnNull && !_request.HasForm)
            {
                return null;
            }

            return _request.Form;
        }

        if (!dontReturnNull && !_request.HasQueryString)
        {
            return null;
        }

        return _request.QueryString;
    }

    private bool DetermineIsExportingWebPart()
    {
        byte[] queryStringBytes = Request.QueryStringBytes;
        if (queryStringBytes == null || queryStringBytes.Length < 28)
        {
            return false;
        }

        if (queryStringBytes[0] != 95 || queryStringBytes[1] != 95 || queryStringBytes[2] != 87 || queryStringBytes[3] != 69 || queryStringBytes[4] != 66 || queryStringBytes[5] != 80 || queryStringBytes[6] != 65 || queryStringBytes[7] != 82 || queryStringBytes[8] != 84 || queryStringBytes[9] != 69 || queryStringBytes[10] != 88 || queryStringBytes[11] != 80 || queryStringBytes[12] != 79 || queryStringBytes[13] != 82 || queryStringBytes[14] != 84 || queryStringBytes[15] != 61 || queryStringBytes[16] != 116 || queryStringBytes[17] != 114 || queryStringBytes[18] != 117 || queryStringBytes[19] != 101 || queryStringBytes[20] != 38)
        {
            return false;
        }

        _pageFlags.Set(2);
        return true;
    }

    //
    // Resumen:
    //     Devuelve un System.Collections.Specialized.NameValueCollection de datos devueltos
    //     a la página con una publicación o un comando GET.
    //
    // Devuelve:
    //     Un System.Collections.Specialized.NameValueCollection objeto que contiene los
    //     datos del formulario. Si la devolución utilizó el comando POST, la información
    //     del formulario se devuelve desde el System.Web.UI.Page.Context objeto. Si la
    //     devolución utilizó el comando GET, se devuelve la información de la cadena de
    //     consulta. Si se solicita la página por primera vez, null se devuelve.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual NameValueCollection DeterminePostBackMode()
    {
        if (Context.Request == null)
        {
            return null;
        }

        if (Context.PreventPostback)
        {
            return null;
        }

        NameValueCollection nameValueCollection = GetCollectionBasedOnMethod(dontReturnNull: false);
        if (nameValueCollection == null)
        {
            return null;
        }

        bool flag = false;
        string[] values = nameValueCollection.GetValues(null);
        if (values != null)
        {
            int num = values.Length;
            for (int i = 0; i < num; i++)
            {
                if (values[i].StartsWith("__VIEWSTATE", StringComparison.Ordinal) || values[i] == "__EVENTTARGET")
                {
                    flag = true;
                    break;
                }
            }
        }

        if (nameValueCollection["__VIEWSTATE"] == null && nameValueCollection["__VIEWSTATEFIELDCOUNT"] == null && nameValueCollection["__EVENTTARGET"] == null && !flag)
        {
            nameValueCollection = null;
        }
        else if (Request.QueryStringText.IndexOf(HttpResponse.RedirectQueryStringAssignment, StringComparison.Ordinal) != -1)
        {
            nameValueCollection = null;
        }

        return nameValueCollection;
    }

    //
    // Resumen:
    //     Devuelve una colección de nombre y valor de datos que se ha registrado en la
    //     página mediante una publicación o un comando GET, sin necesidad de realizar la
    //     validación de solicitudes ASP.NET en la solicitud.
    //
    // Devuelve:
    //     Objeto que contiene los datos del formulario no validados.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual NameValueCollection DeterminePostBackModeUnvalidated()
    {
        if (_request.HttpVerb != HttpVerb.POST)
        {
            return _request.Unvalidated.QueryString;
        }

        return _request.Unvalidated.Form;
    }

    internal static string EncryptString(string s, Purpose purpose)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        byte[] input;
        if (AspNetCryptoServiceProvider.Instance.IsDefaultProvider)
        {
            ICryptoService cryptoService = AspNetCryptoServiceProvider.Instance.GetCryptoService(purpose, CryptoServiceOptions.CacheableOutput);
            input = cryptoService.Protect(bytes);
        }
        else
        {
            input = MachineKeySection.EncryptOrDecryptData(fEncrypt: true, bytes, null, 0, bytes.Length, useValidationSymAlgo: false, useLegacyMode: false, IVType.Hash);
        }

        return HttpServerUtility.UrlTokenEncode(input);
    }

    private void LoadAllState()
    {
        object obj = LoadPageStateFromPersistenceMedium();
        IDictionary dictionary = null;
        Pair pair = null;
        Pair pair2 = obj as Pair;
        if (obj != null)
        {
            dictionary = pair2.First as IDictionary;
            pair = pair2.Second as Pair;
        }

        if (dictionary != null)
        {
            _controlsRequiringPostBack = (ArrayList)dictionary["__ControlsRequirePostBackKey__"];
            if (_registeredControlsRequiringControlState != null)
            {
                foreach (Control item in (IEnumerable)_registeredControlsRequiringControlState)
                {
                    item.LoadControlStateInternal(dictionary[item.UniqueID]);
                }
            }
        }

        if (pair != null)
        {
            string s = (string)pair.First;
            int num = int.Parse(s, NumberFormatInfo.InvariantInfo);
            _fPageLayoutChanged = num != GetTypeHashCode();
            if (!_fPageLayoutChanged)
            {
                LoadViewStateRecursive(pair.Second);
            }
        }
    }

    //
    // Resumen:
    //     Carga cualquier guarda información de estado de vista para la System.Web.UI.Page
    //     objeto.
    //
    // Devuelve:
    //     Estado de vista guardado.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual object LoadPageStateFromPersistenceMedium()
    {
        PageStatePersister pageStatePersister = PageStatePersister;
        try
        {
            pageStatePersister.Load();
        }
        catch (HttpException ex)
        {
            if (_pageFlags[8])
            {
                return null;
            }

            if (ShouldSuppressMacValidationException(ex))
            {
                if (Context != null && Context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Ignoring page state", ex);
                }

                ViewStateMacValidationErrorWasSuppressed = true;
                return null;
            }

            ex.WebEventCode = 3002;
            throw;
        }

        return new Pair(pageStatePersister.ControlState, pageStatePersister.ViewState);
    }

    internal bool ShouldSuppressMacValidationException(Exception e)
    {
        if (!EnableViewStateMacRegistryHelper.SuppressMacValidationErrorsFromCrossPagePostbacks)
        {
            return false;
        }

        if (ViewStateException.IsMacValidationException(e))
        {
            if (EnableViewStateMacRegistryHelper.SuppressMacValidationErrorsAlways)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(ViewStateUserKey))
            {
                return false;
            }

            if (_requestValueCollection == null)
            {
                return true;
            }

            if (!VerifyClientStateIdentifier(_requestValueCollection["__VIEWSTATEGENERATOR"]))
            {
                return true;
            }
        }

        return false;
    }

    private bool VerifyClientStateIdentifier(string identifier)
    {
        if (identifier != null && uint.TryParse(identifier, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
        {
            return result == GetClientStateIdentifier();
        }

        return false;
    }

    internal void LoadScrollPosition()
    {
        if (!(_previousPagePath != null) && _requestValueCollection != null)
        {
            string text = _requestValueCollection["__SCROLLPOSITIONX"];
            double doubleValue;
            if (text != null)
            {
                _scrollPositionX = (HttpUtility.TryParseCoordinates(text, out doubleValue) ? ((int)doubleValue) : 0);
            }

            string text2 = _requestValueCollection["__SCROLLPOSITIONY"];
            if (text2 != null)
            {
                _scrollPositionY = (HttpUtility.TryParseCoordinates(text2, out doubleValue) ? ((int)doubleValue) : 0);
            }
        }
    }

    internal IStateFormatter2 CreateStateFormatter()
    {
        return new ObjectStateFormatter(this, throwOnErrorDeserializing: true);
    }

    internal ICollection DecomposeViewStateIntoChunks()
    {
        string clientState = ClientState;
        if (clientState == null)
        {
            return null;
        }

        if (MaxPageStateFieldLength <= 0)
        {
            ArrayList arrayList = new ArrayList(1);
            arrayList.Add(clientState);
            return arrayList;
        }

        int num = ClientState.Length / MaxPageStateFieldLength;
        ArrayList arrayList2 = new ArrayList(num + 1);
        int num2 = 0;
        for (int i = 0; i < num; i++)
        {
            arrayList2.Add(clientState.Substring(num2, MaxPageStateFieldLength));
            num2 += MaxPageStateFieldLength;
        }

        if (num2 < clientState.Length)
        {
            arrayList2.Add(clientState.Substring(num2));
        }

        if (arrayList2.Count == 0)
        {
            arrayList2.Add(string.Empty);
        }

        return arrayList2;
    }

    internal void RenderViewStateFields(HtmlTextWriter writer)
    {
        if (_hiddenFieldsToRender == null)
        {
            _hiddenFieldsToRender = new Dictionary<string, string>();
        }

        if (ClientState != null)
        {
            ICollection collection = DecomposeViewStateIntoChunks();
            writer.WriteLine();
            if (collection.Count > 1)
            {
                string value = collection.Count.ToString(CultureInfo.InvariantCulture);
                writer.Write("<input type=\"hidden\" name=\"");
                writer.Write("__VIEWSTATEFIELDCOUNT");
                writer.Write("\" id=\"");
                writer.Write("__VIEWSTATEFIELDCOUNT");
                writer.Write("\" value=\"");
                writer.Write(value);
                writer.WriteLine("\" />");
                _hiddenFieldsToRender["__VIEWSTATEFIELDCOUNT"] = value;
            }

            int num = 0;
            foreach (string item in collection)
            {
                writer.Write("<input type=\"hidden\" name=\"");
                string text = "__VIEWSTATE";
                writer.Write("__VIEWSTATE");
                string text2 = null;
                if (num > 0)
                {
                    text2 = num.ToString(CultureInfo.InvariantCulture);
                    text += text2;
                    writer.Write(text2);
                }

                writer.Write("\" id=\"");
                writer.Write(text);
                writer.Write("\" value=\"");
                writer.Write(item);
                writer.WriteLine("\" />");
                num++;
                _hiddenFieldsToRender[text] = item;
            }

            if (EnableViewStateMacRegistryHelper.WriteViewStateGeneratorField)
            {
                ClientScript.RegisterHiddenField("__VIEWSTATEGENERATOR", GetClientStateIdentifier().ToString("X8", CultureInfo.InvariantCulture));
            }
        }
        else
        {
            writer.Write("\r\n<input type=\"hidden\" name=\"");
            writer.Write("__VIEWSTATE");
            writer.Write("\" id=\"");
            writer.Write("__VIEWSTATE");
            writer.WriteLine("\" value=\"\" />");
            _hiddenFieldsToRender["__VIEWSTATE"] = string.Empty;
        }
    }

    internal void BeginFormRender(HtmlTextWriter writer, string formUniqueID)
    {
        bool flag = RenderDivAroundHiddenInputs(writer);
        if (flag)
        {
            writer.WriteLine();
            if (RenderingCompatibility >= VersionUtil.Framework40)
            {
                writer.Write("<div class=\"aspNetHidden\">");
            }
            else
            {
                writer.Write("<div>");
            }
        }

        ClientScript.RenderHiddenFields(writer);
        RenderViewStateFields(writer);
        if (flag)
        {
            writer.WriteLine("</div>");
        }

        if (ClientSupportsJavaScript)
        {
            if (MaintainScrollPositionOnPostBack && !_requireScrollScript)
            {
                ClientScript.RegisterHiddenField("__SCROLLPOSITIONX", _scrollPositionX.ToString(CultureInfo.InvariantCulture));
                ClientScript.RegisterHiddenField("__SCROLLPOSITIONY", _scrollPositionY.ToString(CultureInfo.InvariantCulture));
                ClientScript.RegisterStartupScript(typeof(Page), "PageScrollPositionScript", "\r\ntheForm.oldSubmit = theForm.submit;\r\ntheForm.submit = WebForm_SaveScrollPositionSubmit;\r\n\r\ntheForm.oldOnSubmit = theForm.onsubmit;\r\ntheForm.onsubmit = WebForm_SaveScrollPositionOnSubmit;\r\n" + (IsPostBack ? "\r\ntheForm.oldOnLoad = window.onload;\r\nwindow.onload = WebForm_RestoreScrollPosition;\r\n" : string.Empty), addScriptTags: true);
                RegisterWebFormsScript();
                _requireScrollScript = true;
            }

            if (ClientSupportsFocus && Form != null && (RenderFocusScript || Form.DefaultFocus.Length > 0 || Form.DefaultButton.Length > 0))
            {
                string text = string.Empty;
                if (FocusedControlID.Length > 0)
                {
                    text = FocusedControlID;
                }
                else if (FocusedControl != null)
                {
                    if (FocusedControl.Visible)
                    {
                        text = FocusedControl.ClientID;
                    }
                }
                else if (ValidatorInvalidControl.Length > 0)
                {
                    text = ValidatorInvalidControl;
                }
                else if (LastFocusedControl.Length > 0)
                {
                    text = LastFocusedControl;
                }
                else if (Form.DefaultFocus.Length > 0)
                {
                    text = Form.DefaultFocus;
                }
                else if (Form.DefaultButton.Length > 0)
                {
                    text = Form.DefaultButton;
                }

                if (text.Length > 0 && !CrossSiteScriptingValidation.IsDangerousString(text, out var _) && CrossSiteScriptingValidation.IsValidJavascriptId(text))
                {
                    ClientScript.RegisterClientScriptResource(typeof(HtmlForm), "Focus.js");
                    if (!ClientScript.IsClientScriptBlockRegistered(typeof(HtmlForm), "Focus"))
                    {
                        RegisterWebFormsScript();
                        ClientScript.RegisterStartupScript(typeof(HtmlForm), "Focus", "WebForm_AutoFocus('" + Util.QuoteJScriptString(text) + "');", addScriptTags: true);
                    }

                    ScriptManager?.SetFocusInternal(text);
                }
            }

            if (RenderDisabledControlsScript)
            {
                ClientScript.RegisterOnSubmitStatement(typeof(Page), "PageReEnableControlsScript", "WebForm_ReEnableControls();");
                RegisterWebFormsScript();
            }

            if (_fRequirePostBackScript)
            {
                RenderPostBackScript(writer, formUniqueID);
            }

            if (_fRequireWebFormsScript)
            {
                RenderWebFormsScript(writer);
            }
        }

        ClientScript.RenderClientScriptBlocks(writer);
    }

    internal void EndFormRenderArrayAndExpandoAttribute(HtmlTextWriter writer, string formUniqueID)
    {
        if (!ClientSupportsJavaScript)
        {
            return;
        }

        if (RenderDisabledControlsScript)
        {
            foreach (Control enabledControl in EnabledControls)
            {
                ClientScript.RegisterArrayDeclaration("__enabledControlArray", "'" + enabledControl.ClientID + "'");
            }
        }

        ClientScript.RenderArrayDeclares(writer);
        ClientScript.RenderExpandoAttribute(writer);
    }

    internal void EndFormRenderHiddenFields(HtmlTextWriter writer, string formUniqueID)
    {
        if (RequiresViewStateEncryptionInternal)
        {
            ClientScript.RegisterHiddenField("__VIEWSTATEENCRYPTED", string.Empty);
        }

        if (_containsCrossPagePost)
        {
            string hiddenFieldInitialValue = EncryptString(Request.CurrentExecutionFilePath, Purpose.WebForms_Page_PreviousPageID);
            ClientScript.RegisterHiddenField("__PREVIOUSPAGE", hiddenFieldInitialValue);
        }

        if (EnableEventValidation)
        {
            ClientScript.SaveEventValidationField();
        }

        if (!ClientScript.HasRegisteredHiddenFields)
        {
            return;
        }

        bool flag = RenderDivAroundHiddenInputs(writer);
        if (flag)
        {
            writer.WriteLine();
            if (RenderingCompatibility >= VersionUtil.Framework40)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "aspNetHidden");
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
        }

        ClientScript.RenderHiddenFields(writer);
        if (flag)
        {
            writer.RenderEndTag();
        }
    }

    internal void EndFormRenderPostBackAndWebFormsScript(HtmlTextWriter writer, string formUniqueID)
    {
        if (ClientSupportsJavaScript)
        {
            if (_fRequirePostBackScript && !_fPostBackScriptRendered)
            {
                RenderPostBackScript(writer, formUniqueID);
            }

            if (_fRequireWebFormsScript && !_fWebFormsScriptRendered)
            {
                RenderWebFormsScript(writer);
            }
        }

        ClientScript.RenderClientStartupScripts(writer);
    }

    internal void EndFormRender(HtmlTextWriter writer, string formUniqueID)
    {
        EndFormRenderArrayAndExpandoAttribute(writer, formUniqueID);
        EndFormRenderHiddenFields(writer, formUniqueID);
        EndFormRenderPostBackAndWebFormsScript(writer, formUniqueID);
    }

    internal void OnFormRender()
    {
        if (_fOnFormRenderCalled)
        {
            throw new HttpException(SR.GetString("Multiple_forms_not_allowed"));
        }

        _fOnFormRenderCalled = true;
        _inOnFormRender = true;
    }

    internal void OnFormPostRender(HtmlTextWriter writer)
    {
        _inOnFormRender = false;
        if (_postFormRenderDelegate != null)
        {
            _postFormRenderDelegate(writer, null);
        }
    }

    internal void ResetOnFormRenderCalled()
    {
        _fOnFormRenderCalled = false;
    }

    //
    // Resumen:
    //     Establece el foco del explorador en el control especificado.
    //
    // Parámetros:
    //   control:
    //     Control que recibirá el foco.
    //
    // Excepciones:
    //   T:System.ArgumentNullException:
    //     El valor de control es null.
    //
    //   T:System.InvalidOperationException:
    //     System.Web.UI.Page.SetFocus(System.Web.UI.Control) se llama cuando el control
    //     no forma parte de una página de formularios Web Forms. o bien System.Web.UI.Page.SetFocus(System.Web.UI.Control)
    //     se llama después de la System.Web.UI.Control.PreRender eventos.
    public void SetFocus(Control control)
    {
        if (control == null)
        {
            throw new ArgumentNullException("control");
        }

        if (Form == null)
        {
            throw new InvalidOperationException(SR.GetString("Form_Required_For_Focus"));
        }

        if (Form.ControlState == ControlState.PreRendered)
        {
            throw new InvalidOperationException(SR.GetString("Page_MustCallBeforeAndDuringPreRender", "SetFocus"));
        }

        _focusedControl = control;
        _focusedControlID = null;
        RegisterFocusScript();
    }

    //
    // Resumen:
    //     Establece el foco del explorador en el control con el identificador especificado.
    //
    //
    // Parámetros:
    //   clientID:
    //     Id. del control establecer el foco.
    //
    // Excepciones:
    //   T:System.ArgumentNullException:
    //     El valor de clientID es null.
    //
    //   T:System.InvalidOperationException:
    //     System.Web.UI.Page.SetFocus(System.String) se llama cuando el control no forma
    //     parte de una página de formularios Web Forms. o bien System.Web.UI.Page.SetFocus(System.String)
    //     se llama después de la System.Web.UI.Control.PreRender eventos.
    public void SetFocus(string clientID)
    {
        if (clientID == null || clientID.Trim().Length == 0)
        {
            throw new ArgumentNullException("clientID");
        }

        if (Form == null)
        {
            throw new InvalidOperationException(SR.GetString("Form_Required_For_Focus"));
        }

        if (Form.ControlState == ControlState.PreRendered)
        {
            throw new InvalidOperationException(SR.GetString("Page_MustCallBeforeAndDuringPreRender", "SetFocus"));
        }

        _focusedControlID = clientID.Trim();
        _focusedControl = null;
        RegisterFocusScript();
    }

    internal void SetValidatorInvalidControlFocus(string clientID)
    {
        if (string.IsNullOrEmpty(_validatorInvalidControl))
        {
            _validatorInvalidControl = clientID;
            RegisterFocusScript();
        }
    }

    [SecurityPermission(SecurityAction.Assert, ControlThread = true)]
    internal static void ThreadResetAbortWithAssert()
    {
        Thread.ResetAbort();
    }

    //
    // Resumen:
    //     Devuelve una cadena que se puede utilizar en un evento de cliente para hacer
    //     que la devolución de datos al servidor. La cadena de referencia está definida
    //     por el System.Web.UI.Control objeto.
    //
    // Parámetros:
    //   control:
    //     El control de servidor para procesar la devolución de datos en el servidor.
    //
    // Devuelve:
    //     Cadena que, cuando se trata como script en el cliente, inicia la devolución de
    //     datos.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.GetPostBackEventReference. http://go.microsoft.com/fwlink/?linkid=14202")]
    public string GetPostBackEventReference(Control control)
    {
        return ClientScript.GetPostBackEventReference(control, string.Empty);
    }

    //
    // Resumen:
    //     Devuelve una cadena que se puede utilizar en un evento de cliente para hacer
    //     que la devolución de datos al servidor. La cadena de referencia se define mediante
    //     el control especificado que controla la devolución de datos y un argumento de
    //     cadena de información adicional del evento.
    //
    // Parámetros:
    //   control:
    //     El control de servidor para procesar la devolución de datos.
    //
    //   argument:
    //     El parámetro se pasa al control de servidor.
    //
    // Devuelve:
    //     Cadena que, cuando se trata como script en el cliente, inicia la devolución de
    //     datos.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.GetPostBackEventReference. http://go.microsoft.com/fwlink/?linkid=14202")]
    public string GetPostBackEventReference(Control control, string argument)
    {
        return ClientScript.GetPostBackEventReference(control, argument);
    }

    //
    // Resumen:
    //     Obtiene una referencia que puede utilizarse en un evento de cliente para devolver
    //     datos al servidor para el control especificado y con los argumentos de evento
    //     especificados.
    //
    // Parámetros:
    //   control:
    //     El control de servidor que recibe la devolución de datos de los eventos de cliente.
    //
    //
    //   argument:
    //     System.String que se pasa a System.Web.UI.IPostBackEventHandler.RaisePostBackEvent(System.String).
    //
    //
    // Devuelve:
    //     Cadena que representa el evento de cliente.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.GetPostBackEventReference. http://go.microsoft.com/fwlink/?linkid=14202")]
    public string GetPostBackClientEvent(Control control, string argument)
    {
        return ClientScript.GetPostBackEventReference(control, argument);
    }

    //
    // Resumen:
    //     Obtiene una referencia con javascript: anexado al principio, que puede utilizar
    //     en un evento de cliente para devolver datos al servidor para el control especificado
    //     y con los argumentos de evento especificado.
    //
    // Parámetros:
    //   control:
    //     El control de servidor para procesar la devolución de datos.
    //
    //   argument:
    //     El parámetro se pasa al control de servidor.
    //
    // Devuelve:
    //     Una cadena que representa una llamada de JavaScript a la función de devolución
    //     de datos que incluye los argumentos de evento y el identificador del control
    //     de destino.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.GetPostBackClientHyperlink. http://go.microsoft.com/fwlink/?linkid=14202")]
    public string GetPostBackClientHyperlink(Control control, string argument)
    {
        return ClientScript.GetPostBackClientHyperlink(control, argument, registerForEventValidation: false);
    }

    internal void InitializeStyleSheet()
    {
        if (_pageFlags[1])
        {
            return;
        }

        string styleSheetTheme = StyleSheetTheme;
        if (!string.IsNullOrEmpty(styleSheetTheme))
        {
            BuildResultCompiledType themeBuildResultType = ThemeDirectoryCompiler.GetThemeBuildResultType(Context, styleSheetTheme);
            if (themeBuildResultType == null)
            {
                throw new HttpException(SR.GetString("Page_theme_not_found", styleSheetTheme));
            }

            _styleSheet = (PageTheme)themeBuildResultType.CreateInstance();
            _styleSheet.Initialize(this, styleSheetTheme: true);
        }

        _pageFlags.Set(1);
    }

    private void InitializeThemes()
    {
        string theme = Theme;
        if (!string.IsNullOrEmpty(theme))
        {
            BuildResultCompiledType themeBuildResultType = ThemeDirectoryCompiler.GetThemeBuildResultType(Context, theme);
            if (themeBuildResultType == null)
            {
                throw new HttpException(SR.GetString("Page_theme_not_found", theme));
            }

            _theme = (PageTheme)themeBuildResultType.CreateInstance();
            _theme.Initialize(this, styleSheetTheme: false);
        }
    }

    //
    // Resumen:
    //     Se llama durante la inicialización de la página para crear una colección de contenido
    //     (de controles de contenido) que se pasa a una página maestra, si la página actual
    //     o la página principal hace referencia a una página maestra.
    //
    // Parámetros:
    //   templateName:
    //     El nombre de la plantilla de contenido para agregar.
    //
    //   template:
    //     La plantilla de contenido
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     Ya existe una plantilla de contenido con el mismo nombre.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void AddContentTemplate(string templateName, ITemplate template)
    {
        if (_contentTemplateCollection == null)
        {
            _contentTemplateCollection = new Hashtable(11, StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            _contentTemplateCollection.Add(templateName, template);
        }
        catch (ArgumentException)
        {
            throw new HttpException(SR.GetString("MasterPage_Multiple_content", templateName));
        }
    }

    private void ApplyMasterPage()
    {
        if (Master != null)
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(_masterPageFile.VirtualPathString.ToLower(CultureInfo.InvariantCulture));
            MasterPage.ApplyMasterRecursive(Master, arrayList);
        }
    }

    internal void ApplyControlSkin(Control ctrl)
    {
        if (_theme != null)
        {
            _theme.ApplyControlSkin(ctrl);
        }
    }

    internal bool ApplyControlStyleSheet(Control ctrl)
    {
        if (_styleSheet != null)
        {
            _styleSheet.ApplyControlSkin(ctrl);
            return true;
        }

        return false;
    }

    internal void RegisterFocusScript()
    {
        if (!ClientSupportsFocus || _requireFocusScript)
        {
            return;
        }

        ClientScript.RegisterHiddenField("__LASTFOCUS", string.Empty);
        _requireFocusScript = true;
        if (_partialCachingControlStack == null)
        {
            return;
        }

        foreach (BasePartialCachingControl item in _partialCachingControlStack)
        {
            item.RegisterFocusScript();
        }
    }

    internal void RegisterPostBackScript()
    {
        if (!ClientSupportsJavaScript || _fPostBackScriptRendered)
        {
            return;
        }

        if (!_fRequirePostBackScript)
        {
            ClientScript.RegisterHiddenField("__EVENTTARGET", string.Empty);
            ClientScript.RegisterHiddenField("__EVENTARGUMENT", string.Empty);
            _fRequirePostBackScript = true;
        }

        if (_partialCachingControlStack == null)
        {
            return;
        }

        foreach (BasePartialCachingControl item in _partialCachingControlStack)
        {
            item.RegisterPostBackScript();
        }
    }

    private void RenderPostBackScript(HtmlTextWriter writer, string formUniqueID)
    {
        writer.Write(base.EnableLegacyRendering ? "\r\n<script type=\"text/javascript\">\r\n<!--\r\n" : "\r\n<script type=\"text/javascript\">\r\n//<![CDATA[\r\n");
        if (PageAdapter != null)
        {
            writer.Write("var theForm = ");
            writer.Write(PageAdapter.GetPostBackFormReference(formUniqueID));
            writer.WriteLine(";");
        }
        else
        {
            writer.Write("var theForm = document.forms['");
            writer.Write(formUniqueID);
            writer.WriteLine("'];");
            writer.Write("if (!theForm) {\r\n    theForm = document.");
            writer.Write(formUniqueID);
            writer.WriteLine(";\r\n}");
        }

        writer.WriteLine("function __doPostBack(eventTarget, eventArgument) {\r\n    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {\r\n        theForm.__EVENTTARGET.value = eventTarget;\r\n        theForm.__EVENTARGUMENT.value = eventArgument;\r\n        theForm.submit();\r\n    }\r\n}");
        writer.WriteLine(base.EnableLegacyRendering ? "// -->\r\n</script>\r\n" : "//]]>\r\n</script>\r\n");
        _fPostBackScriptRendered = true;
    }

    internal void RegisterWebFormsScript()
    {
        if (!ClientSupportsJavaScript || _fWebFormsScriptRendered)
        {
            return;
        }

        RegisterPostBackScript();
        _fRequireWebFormsScript = true;
        if (_partialCachingControlStack == null)
        {
            return;
        }

        foreach (BasePartialCachingControl item in _partialCachingControlStack)
        {
            item.RegisterWebFormsScript();
        }
    }

    private void RenderWebFormsScript(HtmlTextWriter writer)
    {
        ClientScript.RenderWebFormsScript(writer);
        _fWebFormsScriptRendered = true;
    }

    //
    // Resumen:
    //     Determina si el bloque de script de cliente con la clave especificada está registrado
    //     con la página.
    //
    // Parámetros:
    //   key:
    //     La clave de cadena del script de cliente para buscar.
    //
    // Devuelve:
    //     true Si el bloque de script está registrado; de lo contrario, false.
    [Obsolete("The recommended alternative is ClientScript.IsClientScriptBlockRegistered(string key). http://go.microsoft.com/fwlink/?linkid=14202")]
    public bool IsClientScriptBlockRegistered(string key)
    {
        return ClientScript.IsClientScriptBlockRegistered(typeof(Page), key);
    }

    //
    // Resumen:
    //     Determina si el script de inicio del cliente está registrado con el System.Web.UI.Page
    //     objeto.
    //
    // Parámetros:
    //   key:
    //     La clave de cadena de la secuencia de comandos de inicio para buscar.
    //
    // Devuelve:
    //     true Si el script de inicio está registrado; de lo contrario, false.
    [Obsolete("The recommended alternative is ClientScript.IsStartupScriptRegistered(string key). http://go.microsoft.com/fwlink/?linkid=14202")]
    public bool IsStartupScriptRegistered(string key)
    {
        return ClientScript.IsStartupScriptRegistered(typeof(Page), key);
    }

    //
    // Resumen:
    //     Declara un valor que se declara como una declaración de matriz de ECMAScript
    //     cuando se procesa la página.
    //
    // Parámetros:
    //   arrayName:
    //     El nombre de la matriz en la que se va a declarar el valor.
    //
    //   arrayValue:
    //     Valor que se va a colocar en la matriz.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.RegisterArrayDeclaration(string arrayName, string arrayValue). http://go.microsoft.com/fwlink/?linkid=14202")]
    public void RegisterArrayDeclaration(string arrayName, string arrayValue)
    {
        ClientScript.RegisterArrayDeclaration(arrayName, arrayValue);
    }

    //
    // Resumen:
    //     Permite a los controles de servidor registren automáticamente un campo oculto
    //     en el formulario. El campo se enviará a la System.Web.UI.Page objeto cuando el
    //     System.Web.UI.HtmlControls.HtmlForm se representa el control de servidor.
    //
    // Parámetros:
    //   hiddenFieldName:
    //     El nombre único del campo oculto para representarse.
    //
    //   hiddenFieldInitialValue:
    //     El valor que se va a emitir en el formulario oculto.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Obsolete("The recommended alternative is ClientScript.RegisterHiddenField(string hiddenFieldName, string hiddenFieldInitialValue). http://go.microsoft.com/fwlink/?linkid=14202")]
    public virtual void RegisterHiddenField(string hiddenFieldName, string hiddenFieldInitialValue)
    {
        ClientScript.RegisterHiddenField(hiddenFieldName, hiddenFieldInitialValue);
    }

    //
    // Resumen:
    //     Emite bloques de script de cliente para la respuesta.
    //
    // Parámetros:
    //   key:
    //     Clave única que identifica un bloque de script.
    //
    //   script:
    //     Contenido del script que se envía al cliente.
    [Obsolete("The recommended alternative is ClientScript.RegisterClientScriptBlock(Type type, string key, string script). http://go.microsoft.com/fwlink/?linkid=14202")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void RegisterClientScriptBlock(string key, string script)
    {
        ClientScript.RegisterClientScriptBlock(typeof(Page), key, script);
    }

    //
    // Resumen:
    //     Emite un bloque de script de cliente en la respuesta de la página.
    //
    // Parámetros:
    //   key:
    //     Clave única que identifica un bloque de script.
    //
    //   script:
    //     Contenido del script que se enviará al cliente.
    [Obsolete("The recommended alternative is ClientScript.RegisterStartupScript(Type type, string key, string script). http://go.microsoft.com/fwlink/?linkid=14202")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void RegisterStartupScript(string key, string script)
    {
        ClientScript.RegisterStartupScript(typeof(Page), key, script, addScriptTags: false);
    }

    //
    // Resumen:
    //     Permite que una página tener acceso el cliente OnSubmit eventos. La secuencia
    //     de comandos debe ser una llamada de función al código de cliente registrado en
    //     otra parte.
    //
    // Parámetros:
    //   key:
    //     Clave única que identifica un bloque de script.
    //
    //   script:
    //     El script de cliente para enviarse al cliente.
    [Obsolete("The recommended alternative is ClientScript.RegisterOnSubmitStatement(Type type, string key, string script). http://go.microsoft.com/fwlink/?linkid=14202")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void RegisterOnSubmitStatement(string key, string script)
    {
        ClientScript.RegisterOnSubmitStatement(typeof(Page), key, script);
    }

    internal void RegisterEnabledControl(Control control)
    {
        EnabledControls.Add(control);
    }

    //
    // Resumen:
    //     Registra un control como un control cuyo estado se debe conservar.
    //
    // Parámetros:
    //   control:
    //     Control que se va a registrar.
    //
    // Excepciones:
    //   T:System.ArgumentException:
    //     El control que se va a registrar es null.
    //
    //   T:System.InvalidOperationException:
    //     La System.Web.UI.Page.RegisterRequiresControlState(System.Web.UI.Control) puede
    //     llamarse al método solamente antes o durante la System.Web.UI.Control.PreRender
    //     eventos.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void RegisterRequiresControlState(Control control)
    {
        if (control == null)
        {
            throw new ArgumentException(SR.GetString("Page_ControlState_ControlCannotBeNull"));
        }

        if (control.ControlState == ControlState.PreRendered)
        {
            throw new InvalidOperationException(SR.GetString("Page_MustCallBeforeAndDuringPreRender", "RegisterRequiresControlState"));
        }

        if (_registeredControlsRequiringControlState == null)
        {
            _registeredControlsRequiringControlState = new ControlSet();
        }

        if (_registeredControlsRequiringControlState.Contains(control))
        {
            return;
        }

        _registeredControlsRequiringControlState.Add(control);
        IDictionary dictionary = (IDictionary)PageStatePersister.ControlState;
        if (dictionary != null)
        {
            string uniqueID = control.UniqueID;
            if (!ControlStateLoadedControlIds.Contains(uniqueID))
            {
                control.LoadControlStateInternal(dictionary[uniqueID]);
                ControlStateLoadedControlIds.Add(uniqueID);
            }
        }
    }

    //
    // Resumen:
    //     Determina si el texto especificado System.Web.UI.Control el objeto está registrado
    //     para participar en la administración del estado de control.
    //
    // Parámetros:
    //   control:
    //     El System.Web.UI.Control para comprobar si un requisito de estado de control.
    //
    //
    // Devuelve:
    //     true Si el texto especificado System.Web.UI.Control requiere control de estado;
    //     en caso contrario, false
    public bool RequiresControlState(Control control)
    {
        if (_registeredControlsRequiringControlState != null)
        {
            return _registeredControlsRequiringControlState.Contains(control);
        }

        return false;
    }

    //
    // Resumen:
    //     Detiene la persistencia del estado de control para el control especificado.
    //
    // Parámetros:
    //   control:
    //     El System.Web.UI.Control para el que se detendrá la persistencia del estado de
    //     control.
    //
    // Excepciones:
    //   T:System.ArgumentException:
    //     El valor de System.Web.UI.Control es null.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void UnregisterRequiresControlState(Control control)
    {
        if (control == null)
        {
            throw new ArgumentException(SR.GetString("Page_ControlState_ControlCannotBeNull"));
        }

        if (_registeredControlsRequiringControlState != null)
        {
            _registeredControlsRequiringControlState.Remove(control);
        }
    }

    internal bool ShouldLoadControlState(Control control)
    {
        if (_registeredControlsRequiringClearChildControlState == null)
        {
            return true;
        }

        foreach (Control key in _registeredControlsRequiringClearChildControlState.Keys)
        {
            if (control != key && control.IsDescendentOf(key))
            {
                return false;
            }
        }

        return true;
    }

    internal void RegisterRequiresClearChildControlState(Control control)
    {
        if (_registeredControlsRequiringClearChildControlState == null)
        {
            _registeredControlsRequiringClearChildControlState = new HybridDictionary();
            _registeredControlsRequiringClearChildControlState.Add(control, true);
        }
        else if (_registeredControlsRequiringClearChildControlState[control] == null)
        {
            _registeredControlsRequiringClearChildControlState.Add(control, true);
        }

        IDictionary dictionary = (IDictionary)PageStatePersister.ControlState;
        if (dictionary == null)
        {
            return;
        }

        List<string> list = new List<string>(dictionary.Count);
        foreach (string key in dictionary.Keys)
        {
            Control control2 = FindControl(key);
            if (control2 != null && control2.IsDescendentOf(control))
            {
                list.Add(key);
            }
        }

        foreach (string item in list)
        {
            dictionary[item] = null;
        }
    }

    //
    // Resumen:
    //     Registra un control como uno que requiere el control de devolución de datos cuando
    //     la página se envía al servidor.
    //
    // Parámetros:
    //   control:
    //     El control se registre.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     El control que se va a registrar no implementa la System.Web.UI.IPostBackDataHandler
    //     interfaz.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void RegisterRequiresPostBack(Control control)
    {
        if (!(control is IPostBackDataHandler))
        {
            IPostBackDataHandler postBackDataHandler = control.AdapterInternal as IPostBackDataHandler;
            if (postBackDataHandler == null)
            {
                throw new HttpException(SR.GetString("Ctrl_not_data_handler"));
            }
        }

        if (_registeredControlsThatRequirePostBack == null)
        {
            _registeredControlsThatRequirePostBack = new ArrayList();
        }

        _registeredControlsThatRequirePostBack.Add(control.UniqueID);
    }

    internal void PushCachingControl(BasePartialCachingControl c)
    {
        if (_partialCachingControlStack == null)
        {
            _partialCachingControlStack = new Stack();
        }

        _partialCachingControlStack.Push(c);
    }

    internal void PopCachingControl()
    {
        _partialCachingControlStack.Pop();
    }

    private void ProcessPostData(NameValueCollection postData, bool fBeforeLoad)
    {
        if (_changedPostDataConsumers == null)
        {
            _changedPostDataConsumers = new ArrayList();
        }

        if (postData != null)
        {
            foreach (string postDatum in postData)
            {
                if (postDatum == null || IsSystemPostField(postDatum))
                {
                    continue;
                }

                Control control = FindControl(postDatum);
                if (control == null)
                {
                    if (fBeforeLoad)
                    {
                        if (_leftoverPostData == null)
                        {
                            _leftoverPostData = new NameValueCollection();
                        }

                        _leftoverPostData.Add(postDatum, null);
                    }

                    continue;
                }

                IPostBackDataHandler postBackDataHandler = control.PostBackDataHandler;
                if (postBackDataHandler == null)
                {
                    if (control.PostBackEventHandler != null)
                    {
                        RegisterRequiresRaiseEvent(control.PostBackEventHandler);
                    }

                    continue;
                }

                if (postBackDataHandler != null)
                {
                    NameValueCollection postCollection = (control.CalculateEffectiveValidateRequest() ? _requestValueCollection : _unvalidatedRequestValueCollection);
                    if (postBackDataHandler.LoadPostData(postDatum, postCollection))
                    {
                        _changedPostDataConsumers.Add(control);
                    }
                }

                if (_controlsRequiringPostBack != null)
                {
                    _controlsRequiringPostBack.Remove(postDatum);
                }
            }
        }

        ArrayList arrayList = null;
        if (_controlsRequiringPostBack == null)
        {
            return;
        }

        foreach (string item in _controlsRequiringPostBack)
        {
            Control control2 = FindControl(item);
            if (control2 != null)
            {
                IPostBackDataHandler postBackDataHandler2 = control2.AdapterInternal as IPostBackDataHandler;
                if (postBackDataHandler2 == null)
                {
                    postBackDataHandler2 = control2 as IPostBackDataHandler;
                }

                if (postBackDataHandler2 == null)
                {
                    throw new HttpException(SR.GetString("Postback_ctrl_not_found", item));
                }

                NameValueCollection postCollection2 = (control2.CalculateEffectiveValidateRequest() ? _requestValueCollection : _unvalidatedRequestValueCollection);
                if (postBackDataHandler2.LoadPostData(item, postCollection2))
                {
                    _changedPostDataConsumers.Add(control2);
                }
            }
            else if (fBeforeLoad)
            {
                if (arrayList == null)
                {
                    arrayList = new ArrayList();
                }

                arrayList.Add(item);
            }
        }

        _controlsRequiringPostBack = arrayList;
    }

    private async Task ProcessPostDataAsync(NameValueCollection postData, bool fBeforeLoad)
    {
        if (_changedPostDataConsumers == null)
        {
            _changedPostDataConsumers = new ArrayList();
        }

        if (postData != null)
        {
            foreach (string postKey in postData)
            {
                if (postKey == null || IsSystemPostField(postKey))
                {
                    continue;
                }

                Control ctrl = null;
                using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    ctrl = FindControl(postKey);
                    await GetWaitForPreviousStepCompletionAwaitable();
                }

                if (ctrl == null)
                {
                    if (fBeforeLoad)
                    {
                        if (_leftoverPostData == null)
                        {
                            _leftoverPostData = new NameValueCollection();
                        }

                        _leftoverPostData.Add(postKey, null);
                    }

                    continue;
                }

                IPostBackDataHandler postBackDataHandler = ctrl.PostBackDataHandler;
                if (postBackDataHandler == null)
                {
                    if (ctrl.PostBackEventHandler != null)
                    {
                        RegisterRequiresRaiseEvent(ctrl.PostBackEventHandler);
                    }

                    continue;
                }

                if (postBackDataHandler != null)
                {
                    NameValueCollection postCollection = (ctrl.CalculateEffectiveValidateRequest() ? _requestValueCollection : _unvalidatedRequestValueCollection);
                    if (await LoadPostDataAsync(postBackDataHandler, postKey, postCollection))
                    {
                        _changedPostDataConsumers.Add(ctrl);
                    }
                }

                if (_controlsRequiringPostBack != null)
                {
                    _controlsRequiringPostBack.Remove(postKey);
                }
            }
        }

        ArrayList leftOverControlsRequiringPostBack = null;
        if (_controlsRequiringPostBack == null)
        {
            return;
        }

        foreach (string postKey in _controlsRequiringPostBack)
        {
            Control ctrl = null;
            using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
            {
                ctrl = FindControl(postKey);
                await GetWaitForPreviousStepCompletionAwaitable();
            }

            if (ctrl != null)
            {
                IPostBackDataHandler postBackDataHandler2 = ctrl.AdapterInternal as IPostBackDataHandler;
                if (postBackDataHandler2 == null)
                {
                    postBackDataHandler2 = ctrl as IPostBackDataHandler;
                }

                if (postBackDataHandler2 == null)
                {
                    throw new HttpException(SR.GetString("Postback_ctrl_not_found", postKey));
                }

                NameValueCollection postCollection2 = (ctrl.CalculateEffectiveValidateRequest() ? _requestValueCollection : _unvalidatedRequestValueCollection);
                if (await LoadPostDataAsync(postBackDataHandler2, postKey, postCollection2))
                {
                    _changedPostDataConsumers.Add(ctrl);
                }
            }
            else if (fBeforeLoad)
            {
                if (leftOverControlsRequiringPostBack == null)
                {
                    leftOverControlsRequiringPostBack = new ArrayList();
                }

                leftOverControlsRequiringPostBack.Add(postKey);
            }
        }

        _controlsRequiringPostBack = leftOverControlsRequiringPostBack;
    }

    private async Task<bool> LoadPostDataAsync(IPostBackDataHandler consumer, string postKey, NameValueCollection postCollection)
    {
        if (AppSettings.EnableAsyncModelBinding && consumer is ListControl)
        {
            ListControl listControl = consumer as ListControl;
            listControl.SkipEnsureDataBoundInLoadPostData = true;
            using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
            {
                listControl.InternalEnsureDataBound();
                await GetWaitForPreviousStepCompletionAwaitable();
            }
        }

        return consumer.LoadPostData(postKey, postCollection);
    }

    internal void RaiseChangedEvents()
    {
        if (_changedPostDataConsumers == null)
        {
            return;
        }

        for (int i = 0; i < _changedPostDataConsumers.Count; i++)
        {
            Control control = (Control)_changedPostDataConsumers[i];
            if (control != null)
            {
                IPostBackDataHandler postBackDataHandler = control.PostBackDataHandler;
                if ((control == null || control.IsDescendentOf(this)) && control != null && control.PostBackDataHandler != null)
                {
                    postBackDataHandler.RaisePostDataChangedEvent();
                }
            }
        }
    }

    internal async Task RaiseChangedEventsAsync()
    {
        if (_changedPostDataConsumers == null)
        {
            return;
        }

        for (int i = 0; i < _changedPostDataConsumers.Count; i++)
        {
            Control control = (Control)_changedPostDataConsumers[i];
            if (control == null)
            {
                continue;
            }

            IPostBackDataHandler postBackDataHandler = control.PostBackDataHandler;
            if ((control == null || control.IsDescendentOf(this)) && control != null && control.PostBackDataHandler != null)
            {
                using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    postBackDataHandler.RaisePostDataChangedEvent();
                    await GetWaitForPreviousStepCompletionAwaitable();
                }
            }
        }
    }

    private void RaisePostBackEvent(NameValueCollection postData)
    {
        if (_registeredControlThatRequireRaiseEvent != null)
        {
            RaisePostBackEvent(_registeredControlThatRequireRaiseEvent, null);
            return;
        }

        string text = postData["__EVENTTARGET"];
        bool flag = !string.IsNullOrEmpty(text);
        if (flag || AutoPostBackControl != null)
        {
            Control control = null;
            if (flag)
            {
                control = FindControl(text);
            }

            if (control != null && control.PostBackEventHandler != null)
            {
                string eventArgument = postData["__EVENTARGUMENT"];
                RaisePostBackEvent(control.PostBackEventHandler, eventArgument);
            }
        }
        else
        {
            Validate();
        }
    }

    //
    // Resumen:
    //     Notifica al control de servidor que realizó la devolución de datos que se debería
    //     controlar un evento de devolución de datos entrante.
    //
    // Parámetros:
    //   sourceControl:
    //     El control de servidor ASP.NET que provocó la devolución de datos. Este control
    //     debe implementar la System.Web.UI.IPostBackEventHandler interfaz.
    //
    //   eventArgument:
    //     El argumento de devolución de datos.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
    {
        sourceControl.RaisePostBackEvent(eventArgument);
    }

    //
    // Resumen:
    //     Registra un control de servidor ASP.NET como uno que requiere un evento que se
    //     genera si el control se procesa en el System.Web.UI.Page objeto.
    //
    // Parámetros:
    //   control:
    //     Control que se va a registrar.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void RegisterRequiresRaiseEvent(IPostBackEventHandler control)
    {
        _registeredControlThatRequireRaiseEvent = control;
    }

    //
    // Resumen:
    //     Recupera la ruta de acceso física que se asigna a una ruta de acceso virtual
    //     absoluta o relativa, o una ruta de acceso relativa a la aplicación.
    //
    // Parámetros:
    //   virtualPath:
    //     Un System.String que representa la ruta de acceso virtual.
    //
    // Devuelve:
    //     La ruta de acceso física asociada a la ruta de acceso virtual o una ruta de acceso
    //     relativa a la aplicación.
    public string MapPath(string virtualPath)
    {
        return _request.MapPath(VirtualPath.CreateAllowNull(virtualPath), base.TemplateControlVirtualDirectory, allowCrossAppMapping: true);
    }

    //
    // Resumen:
    //     Inicializa la caché de resultados para la solicitud de página actual.
    //
    // Parámetros:
    //   duration:
    //     La cantidad de tiempo que los objetos almacenados en la caché de resultados son
    //     válidos.
    //
    //   varyByHeader:
    //     Una lista separada por punto y coma de encabezados que se va a variar el contenido
    //     de la caché de resultados.
    //
    //   varyByCustom:
    //     El Vary encabezado HTTP.
    //
    //   location:
    //     Uno de los valores de System.Web.UI.OutputCacheLocation.
    //
    //   varyByParam:
    //     Una lista separada por comas de los parámetros recibidos por un método GET o
    //     POST que va a variar el contenido de la caché de resultados.
    //
    // Excepciones:
    //   T:System.ArgumentOutOfRangeException:
    //     Se especificó un valor no válido para location.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InitOutputCache(int duration, string varyByHeader, string varyByCustom, OutputCacheLocation location, string varyByParam)
    {
        InitOutputCache(duration, null, varyByHeader, varyByCustom, location, varyByParam);
    }

    //
    // Resumen:
    //     Inicializa la caché de resultados para la solicitud de página actual.
    //
    // Parámetros:
    //   duration:
    //     La cantidad de tiempo que los objetos almacenados en la caché de resultados son
    //     válidos.
    //
    //   varyByContentEncoding:
    //     Una lista separada por comas de juegos de caracteres (codificaciones de contenido)
    //     que el contenido de la caché de resultados varían por.
    //
    //   varyByHeader:
    //     Una lista separada por punto y coma de encabezados que se va a variar el contenido
    //     de la caché de resultados.
    //
    //   varyByCustom:
    //     El Vary encabezado HTTP.
    //
    //   location:
    //     Uno de los valores de System.Web.UI.OutputCacheLocation.
    //
    //   varyByParam:
    //     Una lista separada por comas de los parámetros recibidos por un método GET o
    //     POST que va a variar el contenido de la caché de resultados.
    //
    // Excepciones:
    //   T:System.ArgumentOutOfRangeException:
    //     Se especificó un valor no válido para location.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InitOutputCache(int duration, string varyByContentEncoding, string varyByHeader, string varyByCustom, OutputCacheLocation location, string varyByParam)
    {
        if (!_isCrossPagePostBack)
        {
            OutputCacheParameters outputCacheParameters = new OutputCacheParameters();
            outputCacheParameters.Duration = duration;
            outputCacheParameters.VaryByContentEncoding = varyByContentEncoding;
            outputCacheParameters.VaryByHeader = varyByHeader;
            outputCacheParameters.VaryByCustom = varyByCustom;
            outputCacheParameters.Location = location;
            outputCacheParameters.VaryByParam = varyByParam;
            InitOutputCache(outputCacheParameters);
        }
    }

    //
    // Resumen:
    //     Inicializa la caché de resultados para la solicitud de página actual según un
    //     System.Web.UI.OutputCacheParameters objeto.
    //
    // Parámetros:
    //   cacheSettings:
    //     Un System.Web.UI.OutputCacheParameters que contiene la configuración de caché.
    //
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     No se encontró el perfil de caché. o bien Atributo de perfil de configuración
    //     o falta la directiva.
    //
    //   T:System.ArgumentOutOfRangeException:
    //     La ubicación de configuración de la caché de salida no es válida.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void InitOutputCache(OutputCacheParameters cacheSettings)
    {
        if (_isCrossPagePostBack)
        {
            return;
        }

        OutputCacheProfile outputCacheProfile = null;
        HttpCachePolicy cache = Response.Cache;
        OutputCacheLocation outputCacheLocation = (OutputCacheLocation)(-1);
        int num = 0;
        string text = null;
        string text2 = null;
        string text3 = null;
        string text4 = null;
        string text5 = null;
        string text6 = null;
        bool flag = false;
        RuntimeConfig appConfig = RuntimeConfig.GetAppConfig();
        OutputCacheSection outputCache = appConfig.OutputCache;
        if (!outputCache.EnableOutputCache)
        {
            return;
        }

        if (cacheSettings.CacheProfile != null && cacheSettings.CacheProfile.Length != 0)
        {
            OutputCacheSettingsSection outputCacheSettings = appConfig.OutputCacheSettings;
            outputCacheProfile = outputCacheSettings.OutputCacheProfiles[cacheSettings.CacheProfile];
            if (outputCacheProfile == null)
            {
                throw new HttpException(SR.GetString("CacheProfile_Not_Found", cacheSettings.CacheProfile));
            }

            if (!outputCacheProfile.Enabled)
            {
                return;
            }
        }

        if (outputCacheProfile != null)
        {
            num = outputCacheProfile.Duration;
            text = outputCacheProfile.VaryByContentEncoding;
            text2 = outputCacheProfile.VaryByHeader;
            text3 = outputCacheProfile.VaryByCustom;
            text4 = outputCacheProfile.VaryByParam;
            text5 = outputCacheProfile.SqlDependency;
            flag = outputCacheProfile.NoStore;
            text6 = outputCacheProfile.VaryByControl;
            outputCacheLocation = outputCacheProfile.Location;
            if (string.IsNullOrEmpty(text))
            {
                text = null;
            }

            if (string.IsNullOrEmpty(text2))
            {
                text2 = null;
            }

            if (string.IsNullOrEmpty(text3))
            {
                text3 = null;
            }

            if (string.IsNullOrEmpty(text4))
            {
                text4 = null;
            }

            if (string.IsNullOrEmpty(text6))
            {
                text6 = null;
            }

            if (StringUtil.EqualsIgnoreCase(text4, "none"))
            {
                text4 = null;
            }

            if (StringUtil.EqualsIgnoreCase(text6, "none"))
            {
                text6 = null;
            }
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.Duration))
        {
            num = cacheSettings.Duration;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.VaryByContentEncoding))
        {
            text = cacheSettings.VaryByContentEncoding;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.VaryByHeader))
        {
            text2 = cacheSettings.VaryByHeader;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.VaryByCustom))
        {
            text3 = cacheSettings.VaryByCustom;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.VaryByControl))
        {
            text6 = cacheSettings.VaryByControl;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.VaryByParam))
        {
            text4 = cacheSettings.VaryByParam;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.SqlDependency))
        {
            text5 = cacheSettings.SqlDependency;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.NoStore))
        {
            flag = cacheSettings.NoStore;
        }

        if (cacheSettings.IsParameterSet(OutputCacheParameter.Location))
        {
            outputCacheLocation = cacheSettings.Location;
        }

        if (outputCacheLocation == (OutputCacheLocation)(-1))
        {
            outputCacheLocation = OutputCacheLocation.Any;
        }

        if (outputCacheLocation != OutputCacheLocation.None && (outputCacheProfile == null || outputCacheProfile.Enabled))
        {
            if ((outputCacheProfile == null || outputCacheProfile.Duration == -1) && !cacheSettings.IsParameterSet(OutputCacheParameter.Duration))
            {
                throw new HttpException(SR.GetString("Missing_output_cache_attr", "duration"));
            }

            if ((outputCacheProfile == null || (outputCacheProfile.VaryByParam == null && outputCacheProfile.VaryByControl == null)) && !cacheSettings.IsParameterSet(OutputCacheParameter.VaryByParam) && !cacheSettings.IsParameterSet(OutputCacheParameter.VaryByControl))
            {
                throw new HttpException(SR.GetString("Missing_output_cache_attr", "varyByParam"));
            }
        }

        if (flag)
        {
            Response.Cache.SetNoStore();
        }

        HttpCacheability cacheability;
        switch (outputCacheLocation)
        {
            case OutputCacheLocation.Any:
                cacheability = HttpCacheability.Public;
                break;
            case OutputCacheLocation.Server:
                cacheability = HttpCacheability.Server;
                break;
            case OutputCacheLocation.ServerAndClient:
                cacheability = HttpCacheability.ServerAndPrivate;
                break;
            case OutputCacheLocation.Client:
                cacheability = HttpCacheability.Private;
                break;
            case OutputCacheLocation.Downstream:
                cacheability = HttpCacheability.Public;
                cache.SetNoServerCaching();
                break;
            case OutputCacheLocation.None:
                cacheability = HttpCacheability.NoCache;
                break;
            default:
                throw new ArgumentOutOfRangeException("cacheSettings", SR.GetString("Invalid_cache_settings_location"));
        }

        cache.SetCacheability(cacheability);
        if (outputCacheLocation == OutputCacheLocation.None)
        {
            return;
        }

        cache.SetExpires(Context.Timestamp.AddSeconds(num));
        cache.SetMaxAge(new TimeSpan(0, 0, num));
        cache.SetValidUntilExpires(validUntilExpires: true);
        cache.SetLastModified(Context.Timestamp);
        if (outputCacheLocation == OutputCacheLocation.Client)
        {
            return;
        }

        if (text != null)
        {
            string[] array = text.Split(s_varySeparator);
            string[] array2 = array;
            foreach (string text7 in array2)
            {
                cache.VaryByContentEncodings[text7.Trim()] = true;
            }
        }

        if (text2 != null)
        {
            string[] array3 = text2.Split(s_varySeparator);
            string[] array4 = array3;
            foreach (string text8 in array4)
            {
                cache.VaryByHeaders[text8.Trim()] = true;
            }
        }

        if (PageAdapter != null)
        {
            StringCollection cacheVaryByHeaders = PageAdapter.CacheVaryByHeaders;
            if (cacheVaryByHeaders != null)
            {
                StringEnumerator enumerator = cacheVaryByHeaders.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        cache.VaryByHeaders[current] = true;
                    }
                }
                finally
                {
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        if (outputCacheLocation == OutputCacheLocation.Downstream)
        {
            return;
        }

        if (text3 != null)
        {
            cache.SetVaryByCustom(text3);
        }

        if (string.IsNullOrEmpty(text4) && string.IsNullOrEmpty(text6) && (PageAdapter == null || PageAdapter.CacheVaryByParams == null))
        {
            cache.VaryByParams.IgnoreParams = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(text4))
            {
                string[] array5 = text4.Split(s_varySeparator);
                string[] array6 = array5;
                foreach (string text9 in array6)
                {
                    cache.VaryByParams[text9.Trim()] = true;
                }
            }

            if (!string.IsNullOrEmpty(text6))
            {
                string[] array7 = text6.Split(s_varySeparator);
                string[] array8 = array7;
                foreach (string text10 in array8)
                {
                    cache.VaryByParams[text10.Trim()] = true;
                }
            }

            if (PageAdapter != null)
            {
                IList cacheVaryByParams = PageAdapter.CacheVaryByParams;
                if (cacheVaryByParams != null)
                {
                    foreach (string item in cacheVaryByParams)
                    {
                        cache.VaryByParams[item] = true;
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(text5))
        {
            Response.AddCacheDependency(SqlCacheDependency.CreateOutputCacheDependency(text5));
        }
    }

    //
    // Resumen:
    //     Devuelve una lista de nombres de archivos físicos que corresponden a una lista
    //     de ubicaciones de archivos virtual.
    //
    // Parámetros:
    //   virtualFileDependencies:
    //     Matriz de cadenas de ubicaciones de archivos virtual.
    //
    // Devuelve:
    //     Objeto que contiene una lista de ubicaciones de archivo físico.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected object GetWrappedFileDependencies(string[] virtualFileDependencies)
    {
        return virtualFileDependencies;
    }

    //
    // Resumen:
    //     Agrega una lista de archivos dependientes que componen la página actual. Este
    //     método se usa internamente por el marco de páginas ASP.NET y no está diseñada
    //     para utilizarse directamente desde el código.
    //
    // Parámetros:
    //   virtualFileDependencies:
    //     Un System.Object que contiene la lista de nombres de archivo.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void AddWrappedFileDependencies(object virtualFileDependencies)
    {
        Response.AddVirtualPathDependencies((string[])virtualFileDependencies);
    }

    private CultureInfo CultureFromUserLanguages(bool specific)
    {
        if (_context != null && _context.Request != null && _context.Request.UserLanguages != null)
        {
            try
            {
                return CultureUtil.CreateReadOnlyCulture(_context.Request.UserLanguages, specific);
            }
            catch
            {
            }
        }

        return null;
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.LoadComplete eventos al final de la fase de carga
    //     de página.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected virtual void OnLoadComplete(EventArgs e)
    {
        ((EventHandler)base.Events[EventLoadComplete])?.Invoke(this, e);
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.PreRenderComplete evento después de la System.Web.UI.Page.OnPreRenderComplete(System.EventArgs)
    //     eventos y antes de que se represente la página.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected virtual void OnPreRenderComplete(EventArgs e)
    {
        ((EventHandler)base.Events[EventPreRenderComplete])?.Invoke(this, e);
    }

    private void PerformPreRenderComplete()
    {
        OnPreRenderComplete(EventArgs.Empty);
    }

    //
    // Resumen:
    //     Inicializa el árbol de control durante la generación de la página según la naturaleza
    //     declarativa de la página.
    protected override void FrameworkInitialize()
    {
        base.FrameworkInitialize();
        InitializeStyleSheet();
    }

    //
    // Resumen:
    //     Establece el System.Web.UI.Page.Culture y System.Web.UI.Page.UICulture para el
    //     subproceso actual de la página.
    protected virtual void InitializeCulture()
    {
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Control.Init eventos para inicializar la página.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected internal override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (_theme != null)
        {
            _theme.SetStyleSheet();
        }

        if (_styleSheet != null)
        {
            _styleSheet.SetStyleSheet();
        }
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.PreInit evento al principio de la inicialización
    //     de la página.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected virtual void OnPreInit(EventArgs e)
    {
        ((EventHandler)base.Events[EventPreInit])?.Invoke(this, e);
    }

    private void PerformPreInit()
    {
        OnPreInit(EventArgs.Empty);
        InitializeThemes();
        ApplyMasterPage();
        _preInitWorkComplete = true;
    }

    private async Task PerformPreInitAsync()
    {
        using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
        {
            OnPreInit(EventArgs.Empty);
            await GetWaitForPreviousStepCompletionAwaitable();
        }

        InitializeThemes();
        ApplyMasterPage();
        _preInitWorkComplete = true;
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.InitComplete evento después de la inicialización
    //     de la página.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected virtual void OnInitComplete(EventArgs e)
    {
        ((EventHandler)base.Events[EventInitComplete])?.Invoke(this, e);
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.PreLoad evento después de datos de devolución de
    //     datos se cargan en los controles de servidor de la página pero antes del System.Web.UI.Control.OnLoad(System.EventArgs)
    //     eventos.
    //
    // Parámetros:
    //   e:
    //     Objeto System.EventArgs que contiene los datos del evento.
    protected virtual void OnPreLoad(EventArgs e)
    {
        ((EventHandler)base.Events[EventPreLoad])?.Invoke(this, e);
    }

    //
    // Resumen:
    //     Registra un control con la página como una solicitud de estado de vista cifrado.
    //
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El System.Web.UI.Page.RegisterRequiresViewStateEncryption método debe llamarse
    //     antes o durante la página PreRenderfase en el ciclo de vida de la página.
    public void RegisterRequiresViewStateEncryption()
    {
        if (base.ControlState >= ControlState.PreRendered)
        {
            throw new InvalidOperationException(SR.GetString("Too_late_for_RegisterRequiresViewStateEncryption"));
        }

        _viewStateEncryptionRequested = true;
    }

    //
    // Resumen:
    //     Genera el System.Web.UI.Page.SaveStateComplete evento después de que el estado
    //     de la página se ha guardado en el medio de persistencia.
    //
    // Parámetros:
    //   e:
    //     Un System.EventArgs objeto que contiene los datos del evento.
    protected virtual void OnSaveStateComplete(EventArgs e)
    {
        ((EventHandler)base.Events[EventSaveStateComplete])?.Invoke(this, e);
    }

    //
    // Resumen:
    //     Establece los objetos de servidor intrínsecos de la System.Web.UI.Page objeto,
    //     como la System.Web.UI.Page.Context, System.Web.UI.Page.Request, System.Web.UI.Page.Response,
    //     y System.Web.UI.Page.Application Propiedades.
    //
    // Parámetros:
    //   context:
    //     Un System.Web.HttpContext objeto que proporciona referencias a los objetos de
    //     servidor intrínsecos (por ejemplo, System.Web.HttpContext.Request, System.Web.HttpContext.Response,
    //     y System.Web.HttpContext.Session) utilizados para atender las solicitudes HTTP.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ProcessRequest(HttpContext context)
    {
        if (HttpRuntime.NamedPermissionSet != null && !HttpRuntime.DisableProcessRequestInApplicationTrust)
        {
            if (!HttpRuntime.ProcessRequestInApplicationTrust)
            {
                ProcessRequestWithAssert(context);
                return;
            }

            if (base.NoCompile)
            {
                HttpRuntime.NamedPermissionSet.PermitOnly();
            }
        }

        ProcessRequestWithNoAssert(context);
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    private void ProcessRequestWithAssert(HttpContext context)
    {
        ProcessRequestWithNoAssert(context);
    }

    private void ProcessRequestWithNoAssert(HttpContext context)
    {
        SetIntrinsics(context);
        ProcessRequest();
    }

    [SecurityPermission(SecurityAction.Assert, ControlThread = true)]
    private void SetCultureWithAssert(Thread currentThread, CultureInfo currentCulture, CultureInfo currentUICulture)
    {
        SetCulture(currentThread, currentCulture, currentUICulture);
    }

    private void SetCulture(Thread currentThread, CultureInfo currentCulture, CultureInfo currentUICulture)
    {
        currentThread.CurrentCulture = currentCulture;
        currentThread.CurrentUICulture = currentUICulture;
    }

    private void ProcessRequest()
    {
        Thread currentThread = Thread.CurrentThread;
        CultureInfo currentCulture = currentThread.CurrentCulture;
        CultureInfo currentUICulture = currentThread.CurrentUICulture;
        try
        {
            ProcessRequest(includeStagesBeforeAsyncPoint: true, includeStagesAfterAsyncPoint: true);
        }
        finally
        {
            RestoreCultures(currentThread, currentCulture, currentUICulture);
        }
    }

    private void ProcessRequest(bool includeStagesBeforeAsyncPoint, bool includeStagesAfterAsyncPoint)
    {
        if (includeStagesBeforeAsyncPoint)
        {
            FrameworkInitialize();
            base.ControlState = ControlState.FrameworkInitialized;
        }

        bool flag = Context.WorkerRequest is IIS7WorkerRequest;
        try
        {
            try
            {
                if (IsTransacted)
                {
                    ProcessRequestTransacted();
                }
                else
                {
                    ProcessRequestMain(includeStagesBeforeAsyncPoint, includeStagesAfterAsyncPoint);
                }

                if (includeStagesAfterAsyncPoint)
                {
                    flag = false;
                    ProcessRequestEndTrace();
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    if (flag)
                    {
                        ProcessRequestEndTrace();
                    }
                }
                catch
                {
                }
            }
            finally
            {
                if (includeStagesAfterAsyncPoint)
                {
                    ProcessRequestCleanup();
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private async Task ProcessRequestAsync(bool includeStagesBeforeAsyncPoint, bool includeStagesAfterAsyncPoint)
    {
        if (includeStagesBeforeAsyncPoint)
        {
            FrameworkInitialize();
            base.ControlState = ControlState.FrameworkInitialized;
        }

        bool needToCallEndTrace = Context.WorkerRequest is IIS7WorkerRequest;
        try
        {
            try
            {
                if (!IsTransacted)
                {
                    await ProcessRequestMainAsync(includeStagesBeforeAsyncPoint, includeStagesAfterAsyncPoint).WithinCancellableCallback(Context);
                }
                else
                {
                    ProcessRequestTransacted();
                }

                if (includeStagesAfterAsyncPoint)
                {
                    needToCallEndTrace = false;
                    ProcessRequestEndTrace();
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    if (needToCallEndTrace)
                    {
                        ProcessRequestEndTrace();
                    }
                }
                catch
                {
                }
            }
            finally
            {
                if (includeStagesAfterAsyncPoint)
                {
                    ProcessRequestCleanup();
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private void RestoreCultures(Thread currentThread, CultureInfo prevCulture, CultureInfo prevUICulture)
    {
        if (prevCulture != currentThread.CurrentCulture || prevUICulture != currentThread.CurrentUICulture)
        {
            if (HttpRuntime.IsFullTrust)
            {
                SetCulture(currentThread, prevCulture, prevUICulture);
            }
            else
            {
                SetCultureWithAssert(currentThread, prevCulture, prevUICulture);
            }
        }
    }

    private void ProcessRequestTransacted()
    {
        bool transactionAborted = false;
        TransactedCallback callback = ProcessRequestMain;
        Transactions.InvokeTransacted(callback, (TransactionOption)_transactionMode, ref transactionAborted);
        try
        {
            if (transactionAborted)
            {
                OnAbortTransaction(EventArgs.Empty);
                WebBaseEvent.RaiseSystemEvent(this, 2002);
            }
            else
            {
                OnCommitTransaction(EventArgs.Empty);
                WebBaseEvent.RaiseSystemEvent(this, 2001);
            }

            ValidateRawUrlIfRequired();
        }
        catch (ThreadAbortException)
        {
            throw;
        }
        catch (Exception e)
        {
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_DURING_REQUEST);
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
            if (!HandleError(e))
            {
                throw;
            }
        }
    }

    private void ProcessRequestCleanup()
    {
        if (_request != null)
        {
            _request = null;
            _response = null;
            if (!IsCrossPagePostBack)
            {
                UnloadRecursive(dispose: true);
            }

            if (Context.TraceIsEnabled)
            {
                Trace.StopTracing();
            }
        }
    }

    private void ProcessRequestEndTrace()
    {
        if (Context.TraceIsEnabled)
        {
            Trace.EndRequest();
            if (Trace.PageOutput && !IsCallback && (ScriptManager == null || !ScriptManager.IsInAsyncPostBack))
            {
                Trace.Render(CreateHtmlTextWriter(Response.Output));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }
    }

    internal void SetPreviousPage(Page previousPage)
    {
        _previousPage = previousPage;
    }

    private void ProcessRequestMain()
    {
        ProcessRequestMain(includeStagesBeforeAsyncPoint: true, includeStagesAfterAsyncPoint: true);
    }

    private void ProcessRequestMain(bool includeStagesBeforeAsyncPoint, bool includeStagesAfterAsyncPoint)
    {
        try
        {
            HttpContext context = Context;
            string text = null;
            if (includeStagesBeforeAsyncPoint)
            {
                if (IsInAspCompatMode)
                {
                    AspCompatApplicationStep.OnPageStartSessionObjects();
                }

                if (PageAdapter != null)
                {
                    _requestValueCollection = PageAdapter.DeterminePostBackMode();
                    if (_requestValueCollection != null)
                    {
                        _unvalidatedRequestValueCollection = PageAdapter.DeterminePostBackModeUnvalidated();
                    }
                }
                else
                {
                    _requestValueCollection = DeterminePostBackMode();
                    if (_requestValueCollection != null)
                    {
                        _unvalidatedRequestValueCollection = DeterminePostBackModeUnvalidated();
                    }
                }

                string text2 = string.Empty;
                if (DetermineIsExportingWebPart())
                {
                    if (!RuntimeConfig.GetAppConfig().WebParts.EnableExport)
                    {
                        throw new InvalidOperationException(SR.GetString("WebPartExportHandler_DisabledExportHandler"));
                    }

                    text = Request.QueryString["webPart"];
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new InvalidOperationException(SR.GetString("WebPartExportHandler_InvalidArgument"));
                    }

                    if (string.Equals(Request.QueryString["scope"], "shared", StringComparison.OrdinalIgnoreCase))
                    {
                        _pageFlags.Set(4);
                    }

                    string text3 = Request.QueryString["query"];
                    if (text3 == null)
                    {
                        text3 = string.Empty;
                    }

                    Request.QueryStringText = text3;
                    context.Trace.IsEnabled = false;
                }

                if (_requestValueCollection != null)
                {
                    if (_requestValueCollection["__VIEWSTATEENCRYPTED"] != null)
                    {
                        ContainsEncryptedViewState = true;
                    }

                    text2 = _requestValueCollection["__CALLBACKID"];
                    if (text2 != null && _request.HttpVerb == HttpVerb.POST)
                    {
                        _isCallback = true;
                    }
                    else if (!IsCrossPagePostBack)
                    {
                        VirtualPath virtualPath = null;
                        if (_requestValueCollection["__PREVIOUSPAGE"] != null)
                        {
                            try
                            {
                                virtualPath = VirtualPath.CreateNonRelativeAllowNull(DecryptString(_requestValueCollection["__PREVIOUSPAGE"], Purpose.WebForms_Page_PreviousPageID));
                            }
                            catch
                            {
                                _pageFlags[8] = true;
                            }

                            if (virtualPath != null && virtualPath != Request.CurrentExecutionFilePathObject)
                            {
                                _pageFlags[8] = true;
                                _previousPagePath = virtualPath;
                            }
                        }
                    }
                }

                if (MaintainScrollPositionOnPostBack)
                {
                    LoadScrollPosition();
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreInit");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_INIT_ENTER, _context.WorkerRequest);
                }

                PerformPreInit();
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_INIT_LEAVE, _context.WorkerRequest);
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreInit");
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin Init");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_INIT_ENTER, _context.WorkerRequest);
                }

                InitRecursive(null);
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_INIT_LEAVE, _context.WorkerRequest);
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Init");
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin InitComplete");
                }

                OnInitComplete(EventArgs.Empty);
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End InitComplete");
                }

                if (IsPostBack)
                {
                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin LoadState");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_VIEWSTATE_ENTER, _context.WorkerRequest);
                    }

                    LoadAllState();
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_VIEWSTATE_LEAVE, _context.WorkerRequest);
                    }

                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End LoadState");
                        Trace.Write("aspx.page", "Begin ProcessPostData");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_POSTDATA_ENTER, _context.WorkerRequest);
                    }

                    ProcessPostData(_requestValueCollection, fBeforeLoad: true);
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_POSTDATA_LEAVE, _context.WorkerRequest);
                    }

                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End ProcessPostData");
                    }
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreLoad");
                }

                OnPreLoad(EventArgs.Empty);
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreLoad");
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin Load");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_ENTER, _context.WorkerRequest);
                }

                LoadRecursive();
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_LEAVE, _context.WorkerRequest);
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Load");
                }

                if (IsPostBack)
                {
                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin ProcessPostData Second Try");
                    }

                    ProcessPostData(_leftoverPostData, fBeforeLoad: false);
                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End ProcessPostData Second Try");
                        Trace.Write("aspx.page", "Begin Raise ChangedEvents");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_POST_DATA_CHANGED_ENTER, _context.WorkerRequest);
                    }

                    RaiseChangedEvents();
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_POST_DATA_CHANGED_LEAVE, _context.WorkerRequest);
                    }

                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End Raise ChangedEvents");
                        Trace.Write("aspx.page", "Begin Raise PostBackEvent");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RAISE_POSTBACK_ENTER, _context.WorkerRequest);
                    }

                    RaisePostBackEvent(_requestValueCollection);
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RAISE_POSTBACK_LEAVE, _context.WorkerRequest);
                    }

                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End Raise PostBackEvent");
                    }
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin LoadComplete");
                }

                OnLoadComplete(EventArgs.Empty);
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End LoadComplete");
                }

                if (IsPostBack && IsCallback)
                {
                    PrepareCallback(text2);
                }
                else if (!IsCrossPagePostBack)
                {
                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin PreRender");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_RENDER_ENTER, _context.WorkerRequest);
                    }

                    PreRenderRecursiveInternal();
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_RENDER_LEAVE, _context.WorkerRequest);
                    }

                    if (context.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End PreRender");
                    }
                }
            }

            if (_legacyAsyncInfo == null || _legacyAsyncInfo.CallerIsBlocking)
            {
                ExecuteRegisteredAsyncTasks();
            }

            ValidateRawUrlIfRequired();
            if (!includeStagesAfterAsyncPoint)
            {
                return;
            }

            if (IsCallback)
            {
                RenderCallback();
            }
            else if (!IsCrossPagePostBack)
            {
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreRenderComplete");
                }

                PerformPreRenderComplete();
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreRenderComplete");
                }

                if (context.TraceIsEnabled)
                {
                    BuildPageProfileTree(EnableViewState);
                    Trace.Write("aspx.page", "Begin SaveState");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_SAVE_VIEWSTATE_ENTER, _context.WorkerRequest);
                }

                SaveAllState();
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_SAVE_VIEWSTATE_LEAVE, _context.WorkerRequest);
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End SaveState");
                    Trace.Write("aspx.page", "Begin SaveStateComplete");
                }

                OnSaveStateComplete(EventArgs.Empty);
                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End SaveStateComplete");
                    Trace.Write("aspx.page", "Begin Render");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RENDER_ENTER, _context.WorkerRequest);
                }

                if (text != null)
                {
                    ExportWebPart(text);
                }
                else
                {
                    RenderControl(CreateHtmlTextWriter(Response.Output));
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RENDER_LEAVE, _context.WorkerRequest);
                }

                if (context.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Render");
                }

                CheckRemainingAsyncTasks(isThreadAbort: false);
            }
        }
        catch (ThreadAbortException ex)
        {
            HttpApplication.CancelModuleException ex2 = ex.ExceptionState as HttpApplication.CancelModuleException;
            if (includeStagesBeforeAsyncPoint && includeStagesAfterAsyncPoint && _context.Handler == this && _context.ApplicationInstance != null && ex2 != null && !ex2.Timeout)
            {
                _context.ApplicationInstance.CompleteRequest();
                ThreadResetAbortWithAssert();
                return;
            }

            CheckRemainingAsyncTasks(isThreadAbort: true);
            throw;
        }
        catch (ConfigurationException)
        {
            throw;
        }
        catch (Exception e)
        {
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_DURING_REQUEST);
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
            if (!HandleError(e))
            {
                throw;
            }
        }
    }

    private async Task ProcessRequestMainAsync(bool includeStagesBeforeAsyncPoint, bool includeStagesAfterAsyncPoint)
    {
        _ = 11;
        try
        {
            HttpContext con = Context;
            string exportedWebPartID = null;
            if (includeStagesBeforeAsyncPoint)
            {
                if (IsInAspCompatMode)
                {
                    AspCompatApplicationStep.OnPageStartSessionObjects();
                }

                if (PageAdapter != null)
                {
                    _requestValueCollection = PageAdapter.DeterminePostBackMode();
                    if (_requestValueCollection != null)
                    {
                        _unvalidatedRequestValueCollection = PageAdapter.DeterminePostBackModeUnvalidated();
                    }
                }
                else
                {
                    _requestValueCollection = DeterminePostBackMode();
                    if (_requestValueCollection != null)
                    {
                        _unvalidatedRequestValueCollection = DeterminePostBackModeUnvalidated();
                    }
                }

                string callbackControlId = string.Empty;
                if (DetermineIsExportingWebPart())
                {
                    if (!RuntimeConfig.GetAppConfig().WebParts.EnableExport)
                    {
                        throw new InvalidOperationException(SR.GetString("WebPartExportHandler_DisabledExportHandler"));
                    }

                    exportedWebPartID = Request.QueryString["webPart"];
                    if (string.IsNullOrEmpty(exportedWebPartID))
                    {
                        throw new InvalidOperationException(SR.GetString("WebPartExportHandler_InvalidArgument"));
                    }

                    if (string.Equals(Request.QueryString["scope"], "shared", StringComparison.OrdinalIgnoreCase))
                    {
                        _pageFlags.Set(4);
                    }

                    string text = Request.QueryString["query"];
                    if (text == null)
                    {
                        text = string.Empty;
                    }

                    Request.QueryStringText = text;
                    con.Trace.IsEnabled = false;
                }

                if (_requestValueCollection != null)
                {
                    if (_requestValueCollection["__VIEWSTATEENCRYPTED"] != null)
                    {
                        ContainsEncryptedViewState = true;
                    }

                    callbackControlId = _requestValueCollection["__CALLBACKID"];
                    if (callbackControlId != null && _request.HttpVerb == HttpVerb.POST)
                    {
                        _isCallback = true;
                    }
                    else if (!IsCrossPagePostBack)
                    {
                        VirtualPath virtualPath = null;
                        if (_requestValueCollection["__PREVIOUSPAGE"] != null)
                        {
                            try
                            {
                                virtualPath = VirtualPath.CreateNonRelativeAllowNull(DecryptString(_requestValueCollection["__PREVIOUSPAGE"], Purpose.WebForms_Page_PreviousPageID));
                            }
                            catch
                            {
                                _pageFlags[8] = true;
                            }

                            if (virtualPath != null && virtualPath != Request.CurrentExecutionFilePathObject)
                            {
                                _pageFlags[8] = true;
                                _previousPagePath = virtualPath;
                            }
                        }
                    }
                }

                if (MaintainScrollPositionOnPostBack)
                {
                    LoadScrollPosition();
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreInit");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_INIT_ENTER, _context.WorkerRequest);
                }

                await PerformPreInitAsync().WithinCancellableCallback(con);
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_INIT_LEAVE, _context.WorkerRequest);
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreInit");
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin Init");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_INIT_ENTER, _context.WorkerRequest);
                }

                Task task = InitRecursiveAsync(null, this);
                await task.WithinCancellableCallback(con);
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_INIT_LEAVE, _context.WorkerRequest);
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Init");
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin InitComplete");
                }

                using (con.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    OnInitComplete(EventArgs.Empty);
                    await GetWaitForPreviousStepCompletionAwaitable();
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End InitComplete");
                }

                if (IsPostBack)
                {
                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin LoadState");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_VIEWSTATE_ENTER, _context.WorkerRequest);
                    }

                    LoadAllState();
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_VIEWSTATE_LEAVE, _context.WorkerRequest);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End LoadState");
                        Trace.Write("aspx.page", "Begin ProcessPostData");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_POSTDATA_ENTER, _context.WorkerRequest);
                    }

                    if (AppSettings.EnableAsyncModelBinding)
                    {
                        await ProcessPostDataAsync(_requestValueCollection, fBeforeLoad: true).WithinCancellableCallback(con);
                    }
                    else
                    {
                        ProcessPostData(_requestValueCollection, fBeforeLoad: true);
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_POSTDATA_LEAVE, _context.WorkerRequest);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End ProcessPostData");
                    }
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreLoad");
                }

                using (con.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    OnPreLoad(EventArgs.Empty);
                    await GetWaitForPreviousStepCompletionAwaitable();
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreLoad");
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin Load");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_ENTER, _context.WorkerRequest);
                }

                await LoadRecursiveAsync(this).WithinCancellableCallback(con);
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_LOAD_LEAVE, _context.WorkerRequest);
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Load");
                }

                if (IsPostBack)
                {
                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin ProcessPostData Second Try");
                    }

                    if (AppSettings.EnableAsyncModelBinding)
                    {
                        await ProcessPostDataAsync(_leftoverPostData, fBeforeLoad: false).WithinCancellableCallback(con);
                    }
                    else
                    {
                        ProcessPostData(_leftoverPostData, fBeforeLoad: false);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End ProcessPostData Second Try");
                        Trace.Write("aspx.page", "Begin Raise ChangedEvents");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_POST_DATA_CHANGED_ENTER, _context.WorkerRequest);
                    }

                    await RaiseChangedEventsAsync().WithinCancellableCallback(con);
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_POST_DATA_CHANGED_LEAVE, _context.WorkerRequest);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End Raise ChangedEvents");
                        Trace.Write("aspx.page", "Begin Raise PostBackEvent");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RAISE_POSTBACK_ENTER, _context.WorkerRequest);
                    }

                    using (con.SyncContext.AllowVoidAsyncOperationsBlock())
                    {
                        RaisePostBackEvent(_requestValueCollection);
                        await GetWaitForPreviousStepCompletionAwaitable();
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RAISE_POSTBACK_LEAVE, _context.WorkerRequest);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End Raise PostBackEvent");
                    }
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin LoadComplete");
                }

                using (con.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    OnLoadComplete(EventArgs.Empty);
                    await GetWaitForPreviousStepCompletionAwaitable();
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End LoadComplete");
                }

                if (IsPostBack && IsCallback)
                {
                    await PrepareCallbackAsync(callbackControlId).WithinCancellableCallback(con);
                }
                else if (!IsCrossPagePostBack)
                {
                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "Begin PreRender");
                    }

                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_RENDER_ENTER, _context.WorkerRequest);
                    }

                    await PreRenderRecursiveInternalAsync(this).WithinCancellableCallback(con);
                    if (EtwTrace.IsTraceEnabled(5, 4))
                    {
                        EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_PRE_RENDER_LEAVE, _context.WorkerRequest);
                    }

                    if (con.TraceIsEnabled)
                    {
                        Trace.Write("aspx.page", "End PreRender");
                    }
                }
            }

            if (_legacyAsyncInfo == null || _legacyAsyncInfo.CallerIsBlocking)
            {
                ExecuteRegisteredAsyncTasks();
            }

            ValidateRawUrlIfRequired();
            if (!includeStagesAfterAsyncPoint)
            {
                return;
            }

            if (IsCallback)
            {
                RenderCallback();
            }
            else if (!IsCrossPagePostBack)
            {
                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "Begin PreRenderComplete");
                }

                PerformPreRenderComplete();
                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End PreRenderComplete");
                }

                if (con.TraceIsEnabled)
                {
                    BuildPageProfileTree(EnableViewState);
                    Trace.Write("aspx.page", "Begin SaveState");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_SAVE_VIEWSTATE_ENTER, _context.WorkerRequest);
                }

                SaveAllState();
                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_SAVE_VIEWSTATE_LEAVE, _context.WorkerRequest);
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End SaveState");
                    Trace.Write("aspx.page", "Begin SaveStateComplete");
                }

                OnSaveStateComplete(EventArgs.Empty);
                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End SaveStateComplete");
                    Trace.Write("aspx.page", "Begin Render");
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RENDER_ENTER, _context.WorkerRequest);
                }

                if (exportedWebPartID != null)
                {
                    ExportWebPart(exportedWebPartID);
                }
                else
                {
                    RenderControl(CreateHtmlTextWriter(Response.Output));
                }

                if (EtwTrace.IsTraceEnabled(5, 4))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PAGE_RENDER_LEAVE, _context.WorkerRequest);
                }

                if (con.TraceIsEnabled)
                {
                    Trace.Write("aspx.page", "End Render");
                }

                CheckRemainingAsyncTasks(isThreadAbort: false);
            }
        }
        catch (ThreadAbortException ex)
        {
            HttpApplication.CancelModuleException ex2 = ex.ExceptionState as HttpApplication.CancelModuleException;
            if (includeStagesBeforeAsyncPoint && includeStagesAfterAsyncPoint && _context.Handler == this && _context.ApplicationInstance != null && ex2 != null && !ex2.Timeout)
            {
                _context.ApplicationInstance.CompleteRequest();
                ThreadResetAbortWithAssert();
                return;
            }

            CheckRemainingAsyncTasks(isThreadAbort: true);
            throw;
        }
        catch (ConfigurationException)
        {
            throw;
        }
        catch (Exception e)
        {
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_DURING_REQUEST);
            PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
            if (!HandleError(e))
            {
                throw;
            }
        }
    }

    internal WithinCancellableCallbackTaskAwaitable GetWaitForPreviousStepCompletionAwaitable()
    {
        if (SynchronizationContext.Current is AspNetSynchronizationContext aspNetSynchronizationContext)
        {
            return aspNetSynchronizationContext.WaitForPendingOperationsAsync().WithinCancellableCallback(Context);
        }

        return WithinCancellableCallbackTaskAwaitable.Completed;
    }

    private void BuildPageProfileTree(bool enableViewState)
    {
        if (!_profileTreeBuilt)
        {
            _profileTreeBuilt = true;
            BuildProfileTree("ROOT", enableViewState);
        }
    }

    private void ExportWebPart(string exportedWebPartID)
    {
        //IL_008b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0091: Expected O, but got Unknown
        WebPart webPart = null;
        WebPartManager currentWebPartManager = WebPartManager.GetCurrentWebPartManager(this);
        if (currentWebPartManager != null)
        {
            webPart = currentWebPartManager.WebParts[exportedWebPartID];
        }

        if (webPart == null || webPart.IsClosed || webPart is ProxyWebPart)
        {
            Response.Redirect(Request.RawUrl, endResponse: false);
            return;
        }

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Expires = 0;
        Response.ContentType = "application/mswebpart";
        string text = webPart.DisplayTitle;
        if (string.IsNullOrEmpty(text))
        {
            text = SR.GetString("Part_Untitled");
        }

        NonWordRegex val = new NonWordRegex();
        Response.AddHeader("content-disposition", "attachment; filename=" + ((Regex)(object)val).Replace(text, "") + ".WebPart");
        using XmlTextWriter xmlTextWriter = new XmlTextWriter(Response.Output);
        xmlTextWriter.Formatting = Formatting.Indented;
        xmlTextWriter.WriteStartDocument();
        currentWebPartManager.ExportWebPart(webPart, xmlTextWriter);
        xmlTextWriter.WriteEndDocument();
    }

    private void InitializeWriter(HtmlTextWriter writer)
    {
        if (writer is Html32TextWriter html32TextWriter && Request.Browser != null)
        {
            html32TextWriter.ShouldPerformDivTableSubstitution = Request.Browser.Tables;
        }
    }

    //
    // Resumen:
    //     Inicializa el System.Web.UI.HtmlTextWriter objeto y llama a los controles secundarios
    //     de la System.Web.UI.Page para representar.
    //
    // Parámetros:
    //   writer:
    //     El System.Web.UI.HtmlTextWriter que recibe el contenido de la página.
    protected internal override void Render(HtmlTextWriter writer)
    {
        InitializeWriter(writer);
        base.Render(writer);
    }

    private void PrepareCallback(string callbackControlID)
    {
        Response.Cache.SetNoStore();
        try
        {
            string eventArgument = _requestValueCollection["__CALLBACKPARAM"];
            _callbackControl = FindControl(callbackControlID) as ICallbackEventHandler;
            if (_callbackControl != null)
            {
                _callbackControl.RaiseCallbackEvent(eventArgument);
                return;
            }

            throw new InvalidOperationException(SR.GetString("Page_CallBackTargetInvalid", callbackControlID));
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write('e');
            if (Context.IsCustomErrorEnabled)
            {
                Response.Write(SR.GetString("Page_CallBackError"));
                return;
            }

            bool flag = !string.IsNullOrEmpty(_requestValueCollection["__CALLBACKLOADSCRIPT"]);
            Response.Write(flag ? Util.QuoteJScriptString(HttpUtility.HtmlEncode(ex.Message)) : HttpUtility.HtmlEncode(ex.Message));
        }
    }

    private async Task PrepareCallbackAsync(string callbackControlID)
    {
        Response.Cache.SetNoStore();
        try
        {
            string eventArgument = _requestValueCollection["__CALLBACKPARAM"];
            _callbackControl = FindControl(callbackControlID) as ICallbackEventHandler;
            if (_callbackControl != null)
            {
                using (Context.SyncContext.AllowVoidAsyncOperationsBlock())
                {
                    _callbackControl.RaiseCallbackEvent(eventArgument);
                    await GetWaitForPreviousStepCompletionAwaitable();
                }

                return;
            }

            throw new InvalidOperationException(SR.GetString("Page_CallBackTargetInvalid", callbackControlID));
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write('e');
            if (Context.IsCustomErrorEnabled)
            {
                Response.Write(SR.GetString("Page_CallBackError"));
                return;
            }

            bool flag = !string.IsNullOrEmpty(_requestValueCollection["__CALLBACKLOADSCRIPT"]);
            Response.Write(flag ? Util.QuoteJScriptString(HttpUtility.HtmlEncode(ex.Message)) : HttpUtility.HtmlEncode(ex.Message));
        }
    }

    private void RenderCallback()
    {
        bool flag = !string.IsNullOrEmpty(_requestValueCollection["__CALLBACKLOADSCRIPT"]);
        try
        {
            string text = null;
            if (flag)
            {
                text = _requestValueCollection["__CALLBACKINDEX"];
                if (string.IsNullOrEmpty(text))
                {
                    throw new HttpException(SR.GetString("Page_CallBackInvalid"));
                }

                foreach (char c in text)
                {
                    if (c < '0' || c > '9')
                    {
                        throw new HttpException(SR.GetString("Page_CallBackInvalid"));
                    }
                }

                Response.Write("<script>parent.__pendingCallbacks[");
                Response.Write(text);
                Response.Write("].xmlRequest.responseText=\"");
            }

            if (_callbackControl != null)
            {
                string callbackResult = _callbackControl.GetCallbackResult();
                if (EnableEventValidation)
                {
                    string eventValidationFieldValue = ClientScript.GetEventValidationFieldValue();
                    Response.Write(eventValidationFieldValue.Length.ToString(CultureInfo.InvariantCulture));
                    Response.Write('|');
                    Response.Write(eventValidationFieldValue);
                }
                else
                {
                    Response.Write('s');
                }

                Response.Write(flag ? Util.QuoteJScriptString(callbackResult) : callbackResult);
            }

            if (flag)
            {
                Response.Write("\";parent.__pendingCallbacks[");
                Response.Write(text);
                Response.Write("].xmlRequest.readyState=4;parent.WebForm_CallbackComplete();</script>");
            }
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write('e');
            if (Context.IsCustomErrorEnabled)
            {
                Response.Write(SR.GetString("Page_CallBackError"));
            }
            else
            {
                Response.Write(flag ? Util.QuoteJScriptString(HttpUtility.HtmlEncode(ex.Message)) : HttpUtility.HtmlEncode(ex.Message));
            }
        }
    }

    private bool RenderDivAroundHiddenInputs(HtmlTextWriter writer)
    {
        if (writer.RenderDivAroundHiddenInputs)
        {
            if (base.EnableLegacyRendering)
            {
                return RenderingCompatibility >= VersionUtil.Framework40;
            }

            return true;
        }

        return false;
    }

    internal void SetForm(HtmlForm form)
    {
        _form = form;
    }

    internal void SetPostFormRenderDelegate(RenderMethod renderMethod)
    {
        _postFormRenderDelegate = renderMethod;
    }

    //
    // Resumen:
    //     Provoca el estado de vista de página sea persistente, si se llama.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void RegisterViewStateHandler()
    {
        _needToPersistViewState = true;
    }

    private void SaveAllState()
    {
        if (!_needToPersistViewState)
        {
            return;
        }

        Pair pair = new Pair();
        IDictionary dictionary = null;
        if (_registeredControlsRequiringControlState != null && _registeredControlsRequiringControlState.Count > 0)
        {
            dictionary = new HybridDictionary(_registeredControlsRequiringControlState.Count + 1);
            foreach (Control item in (IEnumerable)_registeredControlsRequiringControlState)
            {
                object obj = item.SaveControlStateInternal();
                if (dictionary[item.UniqueID] == null && obj != null)
                {
                    dictionary.Add(item.UniqueID, obj);
                }
            }
        }

        if (_registeredControlsThatRequirePostBack != null && _registeredControlsThatRequirePostBack.Count > 0)
        {
            if (dictionary == null)
            {
                dictionary = new HybridDictionary();
            }

            dictionary.Add("__ControlsRequirePostBackKey__", _registeredControlsThatRequirePostBack);
        }

        if (dictionary != null && dictionary.Count > 0)
        {
            pair.First = dictionary;
        }

        ViewStateMode viewStateMode = ViewStateMode;
        if (viewStateMode == ViewStateMode.Inherit)
        {
            viewStateMode = ViewStateMode.Enabled;
        }

        Pair pair2 = new Pair(GetTypeHashCode().ToString(NumberFormatInfo.InvariantInfo), SaveViewStateRecursive(viewStateMode));
        if (Context.TraceIsEnabled)
        {
            int viewstateSize = 0;
            if (pair2.Second is Pair)
            {
                viewstateSize = EstimateStateSize(((Pair)pair2.Second).First);
            }
            else if (pair2.Second is Triplet)
            {
                viewstateSize = EstimateStateSize(((Triplet)pair2.Second).First);
            }

            Trace.AddControlStateSize(UniqueID, viewstateSize, (dictionary != null) ? EstimateStateSize(dictionary[UniqueID]) : 0);
        }

        pair.Second = pair2;
        SavePageStateToPersistenceMedium(pair);
    }

    //
    // Resumen:
    //     Guarda cualquier información de estado de vista y el estado de control de la
    //     página.
    //
    // Parámetros:
    //   state:
    //     Un System.Object para almacenar la información de estado de vista.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void SavePageStateToPersistenceMedium(object state)
    {
        PageStatePersister pageStatePersister = PageStatePersister;
        if (state is Pair)
        {
            Pair pair = (Pair)state;
            pageStatePersister.ControlState = pair.First;
            pageStatePersister.ViewState = pair.Second;
        }
        else
        {
            pageStatePersister.ViewState = state;
        }

        pageStatePersister.Save();
    }

    private void SetIntrinsics(HttpContext context)
    {
        SetIntrinsics(context, allowAsync: false);
    }

    private void SetIntrinsics(HttpContext context, bool allowAsync)
    {
        _context = context;
        _request = context.Request;
        _response = context.Response;
        _application = context.Application;
        _cache = context.Cache;
        if (!allowAsync && _context != null && _context.ApplicationInstance != null)
        {
            _context.SyncContext.Disable();
        }

        if (!string.IsNullOrEmpty(_clientTarget))
        {
            _request.ClientTarget = _clientTarget;
        }

        HttpCapabilitiesBase browser = _request.Browser;
        if (browser != null)
        {
            _response.ContentType = browser.PreferredRenderingMime;
            string preferredResponseEncoding = browser.PreferredResponseEncoding;
            string preferredRequestEncoding = browser.PreferredRequestEncoding;
            if (!string.IsNullOrEmpty(preferredResponseEncoding))
            {
                _response.ContentEncoding = Encoding.GetEncoding(preferredResponseEncoding);
            }

            if (!string.IsNullOrEmpty(preferredRequestEncoding))
            {
                _request.ContentEncoding = Encoding.GetEncoding(preferredRequestEncoding);
            }
        }

        HookUpAutomaticHandlers();
    }

    internal void SetHeader(HtmlHead header)
    {
        _header = header;
        if (!string.IsNullOrEmpty(_titleToBeSet))
        {
            if (_header == null)
            {
                throw new InvalidOperationException(SR.GetString("Page_Title_Requires_Head"));
            }

            Title = _titleToBeSet;
            _titleToBeSet = null;
        }

        if (!string.IsNullOrEmpty(_descriptionToBeSet))
        {
            if (_header == null)
            {
                throw new InvalidOperationException(SR.GetString("Page_Description_Requires_Head"));
            }

            MetaDescription = _descriptionToBeSet;
            _descriptionToBeSet = null;
        }

        if (!string.IsNullOrEmpty(_keywordsToBeSet))
        {
            if (_header == null)
            {
                throw new InvalidOperationException(SR.GetString("Page_Description_Requires_Head"));
            }

            MetaKeywords = _keywordsToBeSet;
            _keywordsToBeSet = null;
        }
    }

    internal override void UnloadRecursive(bool dispose)
    {
        base.UnloadRecursive(dispose);
        if (_previousPage != null && _previousPage.IsCrossPagePostBack)
        {
            _previousPage.UnloadRecursive(dispose);
        }
    }

    //
    // Resumen:
    //     Inicia una solicitud de recursos de páginas Active Server (ASP). Este método
    //     se proporciona por compatibilidad con las aplicaciones ASP heredadas.
    //
    // Parámetros:
    //   context:
    //     Un System.Web.HttpContext con información sobre la solicitud actual.
    //
    //   cb:
    //     Método de devolución de llamada.
    //
    //   extraData:
    //     Datos adicionales necesarios para procesar la solicitud de la misma manera que
    //     una solicitud ASP.
    //
    // Devuelve:
    //     Un objeto System.IAsyncResult.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected IAsyncResult AspCompatBeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    {
        SetIntrinsics(context);
        _aspCompatStep = new AspCompatApplicationStep(context, ProcessRequest);
        return _aspCompatStep.BeginAspCompatExecution(cb, extraData);
    }

    //
    // Resumen:
    //     Finaliza una solicitud de recursos de páginas Active Server (ASP). Este método
    //     se proporciona por compatibilidad con las aplicaciones ASP heredadas.
    //
    // Parámetros:
    //   result:
    //     La página ASP generada por la solicitud.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void AspCompatEndProcessRequest(IAsyncResult result)
    {
        _aspCompatStep.EndAspCompatExecution(result);
    }

    //
    // Resumen:
    //     Inicia la ejecución de una tarea asincrónica.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     Hay una excepción en la tarea asincrónica.
    public void ExecuteRegisteredAsyncTasks()
    {
        if (_legacyAsyncTaskManager != null && !_legacyAsyncTaskManager.TaskExecutionInProgress)
        {
            HttpAsyncResult httpAsyncResult = _legacyAsyncTaskManager.ExecuteTasks(null, null);
            if (httpAsyncResult.Error != null)
            {
                throw new HttpException(null, httpAsyncResult.Error);
            }
        }
    }

    private void CheckRemainingAsyncTasks(bool isThreadAbort)
    {
        if (_legacyAsyncTaskManager != null)
        {
            _legacyAsyncTaskManager.DisposeTimer();
            if (isThreadAbort)
            {
                _legacyAsyncTaskManager.CompleteAllTasksNow(syncCaller: true);
            }
            else if (!_legacyAsyncTaskManager.FailedToStartTasks && _legacyAsyncTaskManager.AnyTasksRemain)
            {
                throw new HttpException(SR.GetString("Registered_async_tasks_remain"));
            }
        }
    }

    //
    // Resumen:
    //     Registra una nueva tarea asincrónica con la página.
    //
    // Parámetros:
    //   task:
    //     Un System.Web.UI.PageAsyncTask que define la tarea asincrónica.
    //
    // Excepciones:
    //   T:System.ArgumentNullException:
    //     La tarea asincrónica es null.
    public void RegisterAsyncTask(PageAsyncTask task)
    {
        if (task == null)
        {
            throw new ArgumentNullException("task");
        }

        if (SynchronizationContextUtil.CurrentMode == SynchronizationContextMode.Legacy)
        {
            if (_legacyAsyncTaskManager == null)
            {
                _legacyAsyncTaskManager = new LegacyPageAsyncTaskManager(this);
            }

            LegacyPageAsyncTask task2 = new LegacyPageAsyncTask(task.BeginHandler, task.EndHandler, task.TimeoutHandler, task.State, task.ExecuteInParallel);
            _legacyAsyncTaskManager.AddTask(task2);
            return;
        }

        if (!(this is IHttpAsyncHandler))
        {
            throw new InvalidOperationException(SR.GetString("Async_required"));
        }

        if (_asyncTaskManager == null)
        {
            _asyncTaskManager = new PageAsyncTaskManager();
        }

        IPageAsyncTask pageAsyncTask2;
        if (task.TaskHandler == null)
        {
            IPageAsyncTask pageAsyncTask = new PageAsyncTaskApm(task.BeginHandler, task.EndHandler, task.State);
            pageAsyncTask2 = pageAsyncTask;
        }
        else
        {
            IPageAsyncTask pageAsyncTask = new PageAsyncTaskTap(task.TaskHandler);
            pageAsyncTask2 = pageAsyncTask;
        }

        IPageAsyncTask task3 = pageAsyncTask2;
        _asyncTaskManager.EnqueueTask(task3);
    }

    private void AsyncPageProcessRequestBeforeAsyncPointCancellableCallback(object state)
    {
        ProcessRequest(includeStagesBeforeAsyncPoint: true, includeStagesAfterAsyncPoint: false);
    }

    //
    // Resumen:
    //     Comienza a procesar una solicitud de página asincrónica.
    //
    // Parámetros:
    //   context:
    //     El System.Web.HttpContext para la solicitud.
    //
    //   callback:
    //     El método de devolución de llamada para notificar cuando se completa el proceso.
    //
    //
    //   extraData:
    //     Datos de estado para el método asincrónico.
    //
    // Devuelve:
    //     System.IAsyncResult que hace referencia a la solicitud asincrónica.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected IAsyncResult AsyncPageBeginProcessRequest(HttpContext context, AsyncCallback callback, object extraData)
    {
        if (SynchronizationContextUtil.CurrentMode == SynchronizationContextMode.Legacy)
        {
            return LegacyAsyncPageBeginProcessRequest(context, callback, extraData);
        }

        return TaskAsyncHelper.BeginTask(() => ProcessRequestAsync(context), callback, extraData);
    }

    internal CancellationTokenSource CreateCancellationTokenFromAsyncTimeout()
    {
        TimeSpan asyncTimeout = AsyncTimeout;
        if (!(asyncTimeout <= _maxAsyncTimeout))
        {
            return new CancellationTokenSource();
        }

        return new CancellationTokenSource(asyncTimeout);
    }

    private async Task ProcessRequestAsync(HttpContext context)
    {
        context.SyncContext.ProhibitVoidAsyncOperations();
        SetIntrinsics(context, allowAsync: true);
        if (_asyncTaskManager == null)
        {
            _asyncTaskManager = new PageAsyncTaskManager();
        }

        try
        {
            Task preWorkTask = null;
            _context.InvokeCancellableCallback(delegate
            {
                preWorkTask = ProcessRequestAsync(includeStagesBeforeAsyncPoint: true, includeStagesAfterAsyncPoint: false);
            }, null);
            await preWorkTask;
            try
            {
                using CancellationTokenSource cancellationTokenSource = CreateCancellationTokenFromAsyncTimeout();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                try
                {
                    await _asyncTaskManager.ExecuteTasksAsync(this, EventArgs.Empty, cancellationToken, _context.SyncContext, _context.ApplicationInstance);
                }
                finally
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException(SR.GetString("Async_task_timed_out"));
                    }
                }
            }
            catch (Exception e)
            {
                PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_DURING_REQUEST);
                PerfCounters.IncrementCounter(AppPerfCounter.ERRORS_TOTAL);
                if (!HandleError(e))
                {
                    throw;
                }
            }

            Task postWorkTask = null;
            _context.InvokeCancellableCallback(delegate
            {
                postWorkTask = ProcessRequestAsync(includeStagesBeforeAsyncPoint: false, includeStagesAfterAsyncPoint: true);
            }, null);
            await postWorkTask;
        }
        finally
        {
            ProcessRequestCleanup();
        }
    }

    private IAsyncResult LegacyAsyncPageBeginProcessRequest(HttpContext context, AsyncCallback callback, object extraData)
    {
        SetIntrinsics(context, allowAsync: true);
        if (_legacyAsyncInfo == null)
        {
            _legacyAsyncInfo = new LegacyPageAsyncInfo(this);
        }

        _legacyAsyncInfo.AsyncResult = new HttpAsyncResult(callback, extraData);
        _legacyAsyncInfo.CallerIsBlocking = callback == null;
        try
        {
            _context.InvokeCancellableCallback(AsyncPageProcessRequestBeforeAsyncPointCancellableCallback, null);
        }
        catch (Exception error)
        {
            if (_context.SyncContext.PendingOperationsCount == 0)
            {
                throw;
            }

            _legacyAsyncInfo.SetError(error);
        }

        if (_legacyAsyncTaskManager != null && !_legacyAsyncInfo.CallerIsBlocking)
        {
            _legacyAsyncTaskManager.RegisterHandlersForPagePreRenderCompleteAsync();
        }

        _legacyAsyncInfo.AsyncPointReached = true;
        _context.SyncContext.Disable();
        _legacyAsyncInfo.CallHandlers(onPageThread: true);
        return _legacyAsyncInfo.AsyncResult;
    }

    //
    // Resumen:
    //     Termina de procesar una solicitud de página asincrónica.
    //
    // Parámetros:
    //   result:
    //     Un System.IAsyncResult que hacen referencia a una solicitud asincrónica pendiente.
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void AsyncPageEndProcessRequest(IAsyncResult result)
    {
        if (SynchronizationContextUtil.CurrentMode == SynchronizationContextMode.Legacy)
        {
            LegacyAsyncPageEndProcessRequest(result);
        }
        else
        {
            TaskAsyncHelper.EndTask(result);
        }
    }

    private void LegacyAsyncPageEndProcessRequest(IAsyncResult result)
    {
        if (_legacyAsyncInfo != null)
        {
            _legacyAsyncInfo.AsyncResult.End();
        }
    }

    //
    // Resumen:
    //     Registra a inicial y final delegados de controlador de eventos que no requieren
    //     información de estado para una página asincrónica.
    //
    // Parámetros:
    //   beginHandler:
    //     El delegado de la System.Web.BeginEventHandler (método).
    //
    //   endHandler:
    //     El delegado de la System.Web.EndEventHandler (método).
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El <async> no está establecida la directiva de página en true. o bien El System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)
    //     se invoca después de la System.Web.UI.Control.PreRender eventos.
    //
    //   T:System.ArgumentNullException:
    //     System.Web.UI.PageAsyncTask.BeginHandler o System.Web.UI.PageAsyncTask.EndHandler
    //     es null.
    public void AddOnPreRenderCompleteAsync(BeginEventHandler beginHandler, EndEventHandler endHandler)
    {
        AddOnPreRenderCompleteAsync(beginHandler, endHandler, null);
    }

    //
    // Resumen:
    //     Registros de inicio y fin de los delegados de controladores de eventos para una
    //     página asincrónica.
    //
    // Parámetros:
    //   beginHandler:
    //     El delegado de la System.Web.BeginEventHandler (método).
    //
    //   endHandler:
    //     El delegado de la System.Web.EndEventHandler (método).
    //
    //   state:
    //     Objeto que contiene información de estado de los controladores de eventos.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     El <async> no está establecida la directiva de página en true. o bien El System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)
    //     se invoca después de la System.Web.UI.Control.PreRender eventos.
    //
    //   T:System.ArgumentNullException:
    //     System.Web.UI.PageAsyncTask.BeginHandler o System.Web.UI.PageAsyncTask.EndHandler
    //     es null.
    public void AddOnPreRenderCompleteAsync(BeginEventHandler beginHandler, EndEventHandler endHandler, object state)
    {
        if (beginHandler == null)
        {
            throw new ArgumentNullException("beginHandler");
        }

        if (endHandler == null)
        {
            throw new ArgumentNullException("endHandler");
        }

        if (SynchronizationContextUtil.CurrentMode == SynchronizationContextMode.Normal)
        {
            RegisterAsyncTask(new PageAsyncTask(beginHandler, endHandler, null, state));
            return;
        }

        if (_legacyAsyncInfo == null)
        {
            if (!(this is IHttpAsyncHandler))
            {
                throw new InvalidOperationException(SR.GetString("Async_required"));
            }

            _legacyAsyncInfo = new LegacyPageAsyncInfo(this);
        }

        if (_legacyAsyncInfo.AsyncPointReached)
        {
            throw new InvalidOperationException(SR.GetString("Async_addhandler_too_late"));
        }

        _legacyAsyncInfo.AddHandler(beginHandler, endHandler, state);
    }

    //
    // Resumen:
    //     Indica a los controles de validación que se incluye en la página que validen
    //     la información asignada.
    public virtual void Validate()
    {
        _validated = true;
        if (_validators != null)
        {
            for (int i = 0; i < Validators.Count; i++)
            {
                Validators[i].Validate();
            }
        }
    }

    //
    // Resumen:
    //     Indica a los controles de validación en el grupo de validación especificado que
    //     validen la información asignada.
    //
    // Parámetros:
    //   validationGroup:
    //     El nombre del grupo de validación de los controles que se va a validar.
    public virtual void Validate(string validationGroup)
    {
        _validated = true;
        if (_validators == null)
        {
            return;
        }

        ValidatorCollection validators = GetValidators(validationGroup);
        if (string.IsNullOrEmpty(validationGroup) && _validators.Count == validators.Count)
        {
            Validate();
            return;
        }

        for (int i = 0; i < validators.Count; i++)
        {
            validators[i].Validate();
        }
    }

    //
    // Resumen:
    //     Devuelve una colección de validadores de control para un grupo de validación
    //     especificado.
    //
    // Parámetros:
    //   validationGroup:
    //     El grupo de validación para devolver, o null para devolver el grupo de validación
    //     de forma predeterminada.
    //
    // Devuelve:
    //     Un System.Web.UI.ValidatorCollection que contiene los validadores de control
    //     para el grupo de validación especificado.
    public ValidatorCollection GetValidators(string validationGroup)
    {
        if (validationGroup == null)
        {
            validationGroup = string.Empty;
        }

        ValidatorCollection validatorCollection = new ValidatorCollection();
        if (_validators != null)
        {
            for (int i = 0; i < Validators.Count; i++)
            {
                if (Validators[i] is BaseValidator baseValidator)
                {
                    if (string.Compare(baseValidator.ValidationGroup, validationGroup, StringComparison.Ordinal) == 0)
                    {
                        validatorCollection.Add(baseValidator);
                    }
                }
                else if (validationGroup.Length == 0)
                {
                    validatorCollection.Add(Validators[i]);
                }
            }
        }

        return validatorCollection;
    }

    //
    // Resumen:
    //     Confirma que un System.Web.UI.HtmlControls.HtmlForm control se representa para
    //     el control de servidor ASP.NET especificado en tiempo de ejecución.
    //
    // Parámetros:
    //   control:
    //     El control de servidor ASP.NET que se requiere en el System.Web.UI.HtmlControls.HtmlForm
    //     control.
    //
    // Excepciones:
    //   T:System.Web.HttpException:
    //     El control de servidor especificado no está incluido entre las etiquetas apertura
    //     y cierre de la System.Web.UI.HtmlControls.HtmlForm el control de servidor en
    //     tiempo de ejecución.
    //
    //   T:System.ArgumentNullException:
    //     El control que se va a comprobar es null.
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void VerifyRenderingInServerForm(Control control)
    {
        if (Context != null && !base.DesignMode)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            if (!_inOnFormRender && !IsCallback)
            {
                throw new HttpException(SR.GetString("ControlRenderedOutsideServerForm", control.ClientID, control.GetType().Name));
            }
        }
    }

    internal bool GetDesignModeInternal()
    {
        if (!_designModeChecked)
        {
            _designMode = base.Site != null && base.Site.DesignMode;
            _designModeChecked = true;
        }

        return _designMode;
    }

    internal void PushDataBindingContext(object dataItem)
    {
        if (_dataBindingContext == null)
        {
            _dataBindingContext = new Stack();
        }

        _dataBindingContext.Push(dataItem);
    }

    internal void PopDataBindingContext()
    {
        _dataBindingContext.Pop();
    }

    //
    // Resumen:
    //     Obtiene el elemento de datos en la parte superior de la pila de contexto de enlace
    //     de datos.
    //
    // Devuelve:
    //     El objeto en la parte superior de la pila de contexto de enlace de datos.
    //
    // Excepciones:
    //   T:System.InvalidOperationException:
    //     No hay ningún contexto de enlace de datos para la página.
    public object GetDataItem()
    {
        if (_dataBindingContext == null || _dataBindingContext.Count == 0)
        {
            throw new InvalidOperationException(SR.GetString("Page_MissingDataBindingContext"));
        }

        return _dataBindingContext.Peek();
    }

    internal static bool IsSystemPostField(string field)
    {
        return s_systemPostFields.Contains(field);
    }

    private void ValidateRawUrlIfRequired()
    {
        if (!SkipFormActionValidation && CalculateEffectiveValidateRequest())
        {
            string rawUrl = _request.RawUrl;
        }
    }
}
#if false // Registro de descompilación
"30" elementos en caché
------------------
Resolver: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Se encontró un solo ensamblado: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll"
------------------
Resolver: "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Se encontró un solo ensamblado: "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Drawing.dll"
------------------
Resolver: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Se encontró un solo ensamblado: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.dll"
------------------
Resolver: "System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Se encontró un solo ensamblado: "System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Configuration.dll"
------------------
Resolver: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Se encontró un solo ensamblado: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Core.dll"
------------------
Resolver: "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Se encontró un solo ensamblado: "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Data.dll"
------------------
Resolver: "System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Se encontró un solo ensamblado: "System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Xml.dll"
------------------
Resolver: "System.DirectoryServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.DirectoryServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Se encontró un solo ensamblado: "System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.EnterpriseServices.dll"
------------------
Resolver: "System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Se encontró un solo ensamblado: "System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Web.ApplicationServices.dll"
------------------
Resolver: "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Se encontró un solo ensamblado: "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.ComponentModel.DataAnnotations.dll"
------------------
Resolver: "System.DirectoryServices.Protocols, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.DirectoryServices.Protocols, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.ServiceProcess, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.ServiceProcess, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Se encontró un solo ensamblado: "System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Cargar desde: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Web.Services.dll"
------------------
Resolver: "Microsoft.Build.Utilities.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "Microsoft.Build.Utilities.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "Microsoft.Build.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "Microsoft.Build.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
No se pudo encontrar por el nombre "Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Resolver: "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
No se pudo encontrar por el nombre "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
#endif
