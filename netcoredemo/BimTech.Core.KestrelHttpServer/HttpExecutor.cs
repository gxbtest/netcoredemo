using BimTech.Core.CPlatform.Convertibles;
using BimTech.Core.CPlatform.Filters;
using BimTech.Core.CPlatform.Messages;
using BimTech.Core.CPlatform.Routing;
using BimTech.Core.CPlatform.Runtime.Server;
using BimTech.Core.CPlatform.Transport;
using BimTech.Core.ProxyGenerator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BimTech.Core.CPlatform.Utilities.FastInvoke;

namespace BimTech.Core.KestrelHttpServer
{
    public class HttpExecutor : IServiceExecutor
    {
        #region Field
        private readonly IServiceEntryLocate _serviceEntryLocate;
        private readonly IServiceRouteProvider _serviceRouteProvider;
        private readonly IAuthorizationFilter _authorizationFilter;
        private readonly ITypeConvertibleService _typeConvertibleService;
        private readonly IServiceProxyProvider _serviceProxyProvider;
        private readonly ConcurrentDictionary<string, ValueTuple<FastInvokeHandler, object, MethodInfo>> _concurrent =
        new ConcurrentDictionary<string, ValueTuple<FastInvokeHandler, object, MethodInfo>>();
        private readonly DiagnosticListener _diagnosticListener;
        #endregion Field

        #region Constructor

        public HttpExecutor(IServiceEntryLocate serviceEntryLocate, IServiceRouteProvider serviceRouteProvider,
            IAuthorizationFilter authorizationFilter,
             ITypeConvertibleService typeConvertibleService)
        {
            _serviceEntryLocate = serviceEntryLocate;
            _typeConvertibleService = typeConvertibleService;
            _serviceRouteProvider = serviceRouteProvider;
            _authorizationFilter = authorizationFilter;
            //_serviceProxyProvider = serviceProxyProvider;
            _diagnosticListener = new DiagnosticListener("BimTechDiagnosticListener");
        }
        #endregion Constructor

        #region Implementation of IExecutor

        public async Task ExecuteAsync(IMessageSender sender, TransportMessage message)
        {

            if (!message.IsHttpMessage())
                return;
            HttpMessage httpMessage;
            try
            {
                httpMessage = message.GetContent<HttpMessage>();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message+"将接收到的消息反序列化成 TransportMessage<httpMessage> 时发送了错误。");
                return;
            }
            //if (httpMessage.Attachments != null)
            //{
            //    foreach (var attachment in httpMessage.Attachments)
            //        RpcContext.GetContext().SetAttachment(attachment.Key, attachment.Value);
            //}
            WirteDiagnosticBefore(message);
            var entry = _serviceEntryLocate.Locate(httpMessage);

            HttpResultMessage<object> httpResultMessage = new HttpResultMessage<object>() { };

            if (entry != null)
            {
                //执行本地代码。
                httpResultMessage = await LocalExecuteAsync(entry, httpMessage);
            }
            else
            {
                //httpResultMessage = await RemoteExecuteAsync(httpMessage);
            }
            await SendRemoteInvokeResult(sender, message.Id, httpResultMessage);
        }


        #endregion Implementation of IServiceExecutor

        #region Private Method

        private async Task<HttpResultMessage<object>> RemoteExecuteAsync(HttpMessage httpMessage)
        {
            HttpResultMessage<object> resultMessage = new HttpResultMessage<object>();
            try
            {
                resultMessage.Entity = await _serviceProxyProvider.Invoke<object>(httpMessage.Parameters, httpMessage.RoutePath, httpMessage.ServiceKey);
                resultMessage.IsSucceed = resultMessage.Entity != default;
                resultMessage.StatusCode = resultMessage.IsSucceed ? (int)StatusCode.Success : (int)StatusCode.RequestError;
            }
            catch (Exception ex)
            {
                resultMessage = new HttpResultMessage<object> { Entity = null, Message = "执行发生了错误。", StatusCode = (int)StatusCode.RequestError };
            }
            return resultMessage;
        }

        private async Task<HttpResultMessage<object>> LocalExecuteAsync(ServiceEntry entry, HttpMessage httpMessage)
        {
            HttpResultMessage<object> resultMessage = new HttpResultMessage<object>();
            try
            {
                var result = await entry.Func(httpMessage.ServiceKey, httpMessage.Parameters);
                var task = result as Task;

                if (task == null)
                {
                    resultMessage.Entity = result;
                }
                else
                {
                    task.Wait();
                    var taskType = task.GetType().GetTypeInfo();
                    if (taskType.IsGenericType)
                        resultMessage.Entity = taskType.GetProperty("Result").GetValue(task);
                }

                resultMessage.IsSucceed = resultMessage.Entity != null;
                resultMessage.StatusCode =
                    resultMessage.IsSucceed ? (int)StatusCode.Success : (int)StatusCode.RequestError;
            }
            catch (Exception ex)
            {
                Console.WriteLine("执行本地逻辑时候发生了错误。");

                resultMessage.Message = ex.Message;
                resultMessage.StatusCode = ex.HResult;
            }
            return resultMessage;
        }

        private async Task SendRemoteInvokeResult(IMessageSender sender, string messageId, HttpResultMessage resultMessage)
        {
            try
            {

                await sender.SendAndFlushAsync(new TransportMessage(messageId, resultMessage));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message+"发送响应消息时候发生了异常。");
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var message = exception.Message;
            if (exception.InnerException != null)
            {
                message += "|InnerException:" + GetExceptionMessage(exception.InnerException);
            }
            return message;
        }

        private void WirteDiagnosticBefore(TransportMessage message)
        {
            //if (!AppConfig.ServerOptions.DisableDiagnostic)
            //{
            //    RpcContext.GetContext().SetAttachment("TraceId", message.Id);
            //    var remoteInvokeMessage = message.GetContent<HttpMessage>();
            //    _diagnosticListener.WriteTransportBefore(TransportType.Rest, new TransportEventData(new DiagnosticMessage
            //    {
            //        Content = message.Content,
            //        ContentType = message.ContentType,
            //        Id = message.Id,
            //        MessageName = remoteInvokeMessage.RoutePath
            //    }, TransportType.Rest.ToString(),
            //   message.Id,
            //    RestContext.GetContext().GetAttachment("RemoteIpAddress")?.ToString()));
            //}
            //else
            //{
            //    var parameters = RpcContext.GetContext().GetContextParameters();
            //    RpcContext.GetContext().SetContextParameters(parameters);
            //}

        }

        #endregion Private Method

    }
}
