using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.ErrorHandling;

/// <summary>
/// Методы расширения для создания сведений о неудачных запросах
/// </summary>
public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Регистрирует стандартную обработку ошибок с использованием ProblemDetails
    /// </summary>
    /// <remarks>
    /// <para>
    /// Обертка над стандартным
    /// <see cref="ProblemDetailsServiceCollectionExtensions.AddProblemDetails(IServiceCollection)"/>
    /// </para>
    /// <para>
    /// При использовании MVC контроллеров обязательно вызвать
    /// <see cref="UseErrorHandling(WebApplication)"/>
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddDefaultErrorHandling(this IHostApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        return builder;
    }

    /// <summary>
    /// Регистрирует расширенную обработку ошибок с дополнительной отладочной информацией в Development среде
    /// </summary>
    /// <remarks>
    /// <para>
    /// Добавляет к стандартному сообщению свойство "exception", которое содержит:
    /// </para>
    /// <list type="bullet">
    /// <item>type - полное наименование типа исключения</item>
    /// <item>message - сообщение исключения</item>
    /// <item>stackTrace - stack trace исключения</item>
    /// </list>
    /// <para>Дополнительное поле будет добавлено только в Development среде.
    /// Иначе будет возвращен объект, соответствующий RFC 7807</para>
    /// <para>
    /// ⚠️ <strong>Внимание:</strong> Не используйте в Production среде, так как stack trace
    /// может содержать конфиденциальную информацию
    /// </para>
    /// <para>
    /// При использовании MVC контроллеров обязательно вызвать
    /// <see cref="UseErrorHandling(WebApplication)"/>
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddExtendedErrorHandling(this IHostApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                if (builder.Environment.IsDevelopment() && context.Exception is not null)
                {
                    context.ProblemDetails.Extensions["exception"] = new
                    {
                        type = context.Exception.GetType().FullName,
                        message = context.Exception.Message,
                        stackTrace = context.Exception.StackTrace
                    };
                }
            };
        });

        return builder;
    }

    /// <summary>
    /// Подключает промежуточное ПО для обработки исключений в конвейере запросов
    /// </summary>
    /// <remarks>
    /// <para>Обертка над
    /// <see cref="ExceptionHandlerExtensions.UseExceptionHandler(IApplicationBuilder)"/></para>
    /// <para>Обязательно должен быть вызван при использовании MVC контроллеров</para>
    /// <para>При использовании Minimal Api метод app.UseExceptionHandler() будет вызван автоматически.
    /// Поэтому метод UseErrorHandling() допустимо не вызывать</para>
    /// </remarks>
    public static WebApplication UseErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }

}
