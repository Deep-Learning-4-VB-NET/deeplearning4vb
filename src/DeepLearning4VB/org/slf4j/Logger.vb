Imports System
Imports System.Diagnostics
Imports System.Text

Namespace org.slf4j

    ''' <summary>Log levels, in increasing order of severity.</summary>
    Public Enum LogLevel
        Trace = 0
        Debug = 1
        Info = 2
        Warn = 3
        [Error] = 4
        Off = 5
    End Enum

    ''' <summary>
    ''' A minimal logging interface with the shape of the SLF4J <c>Logger</c> that the Java
    ''' sources use. Messages use SLF4J style <c>{}</c> placeholders. Applications can plug in
    ''' their own implementation through <see cref="LoggerFactory.Provider"/>.
    ''' </summary>
    Public Interface Logger
        ReadOnly Property Name As String
        ReadOnly Property IsTraceEnabled As Boolean
        ReadOnly Property IsDebugEnabled As Boolean
        ReadOnly Property IsInfoEnabled As Boolean
        ReadOnly Property IsWarnEnabled As Boolean
        ReadOnly Property IsErrorEnabled As Boolean

        Sub trace(ByVal format As String, ByVal ParamArray args() As Object)
        Sub debug(ByVal format As String, ByVal ParamArray args() As Object)
        Sub info(ByVal format As String, ByVal ParamArray args() As Object)
        Sub warn(ByVal format As String, ByVal ParamArray args() As Object)
        Sub [error](ByVal format As String, ByVal ParamArray args() As Object)
        Sub [error](ByVal message As String, ByVal exception As Exception)
    End Interface

    ''' <summary>Creates <see cref="Logger"/> instances.</summary>
    Public NotInheritable Class LoggerFactory
        Private Shared providerFunc As Func(Of String, Logger) = Function(name) New TraceLogger(name)
        Private Shared levelValue As LogLevel = LogLevel.Info

        Private Sub New()
        End Sub

        ''' <summary>The factory used to create loggers. Replace it to route log output elsewhere.</summary>
        Public Shared Property Provider As Func(Of String, Logger)
            Get
                Return providerFunc
            End Get
            Set(ByVal value As Func(Of String, Logger))
                providerFunc = If(value, Function(name) New TraceLogger(name))
            End Set
        End Property

        ''' <summary>Threshold applied by the built-in <see cref="TraceLogger"/>. Defaults to Info.</summary>
        Public Shared Property Level As LogLevel
            Get
                Return levelValue
            End Get
            Set(ByVal value As LogLevel)
                levelValue = value
            End Set
        End Property

        Public Shared Function getLogger(ByVal name As String) As Logger
            Return providerFunc(If(name, "root"))
        End Function

        Public Shared Function getLogger(ByVal type As Type) As Logger
            Return getLogger(If(type Is Nothing, "root", type.FullName))
        End Function
    End Class

    ''' <summary>Formats SLF4J style messages: each <c>{}</c> is replaced by the next argument.</summary>
    Public NotInheritable Class MessageFormatter
        Private Sub New()
        End Sub

        Public Shared Function format(ByVal pattern As String, ByVal ParamArray args() As Object) As String
            If pattern Is Nothing Then Return ""
            If args Is Nothing OrElse args.Length = 0 Then Return pattern
            Dim sb As New StringBuilder(pattern.Length + 32)
            Dim argIndex As Integer = 0
            Dim i As Integer = 0
            While i < pattern.Length
                If pattern(i) = "{"c AndAlso i + 1 < pattern.Length AndAlso pattern(i + 1) = "}"c Then
                    If i > 0 AndAlso pattern(i - 1) = "\"c Then
                        sb.Length -= 1
                        sb.Append("{}")
                    ElseIf argIndex < args.Length Then
                        sb.Append(Arrays.FormatElement(args(argIndex)))
                        argIndex += 1
                    Else
                        sb.Append("{}")
                    End If
                    i += 2
                Else
                    sb.Append(pattern(i))
                    i += 1
                End If
            End While
            ' SLF4J convention: a trailing exception that was not consumed by a placeholder is appended.
            If argIndex < args.Length AndAlso TypeOf args(args.Length - 1) Is Exception Then
                sb.Append(Environment.NewLine).Append(DirectCast(args(args.Length - 1), Exception).ToString())
            End If
            Return sb.ToString()
        End Function
    End Class

    ''' <summary>Default logger: writes to <see cref="System.Diagnostics.Trace"/> (visible in the VS Output window).</summary>
    Public Class TraceLogger
        Implements Logger

        Private ReadOnly loggerName As String

        Public Sub New(ByVal name As String)
            loggerName = name
        End Sub

        Public ReadOnly Property Name As String Implements Logger.Name
            Get
                Return loggerName
            End Get
        End Property

        Private Shared Function Enabled(ByVal level As LogLevel) As Boolean
            Return level >= LoggerFactory.Level
        End Function

        Public ReadOnly Property IsTraceEnabled As Boolean Implements Logger.IsTraceEnabled
            Get
                Return Enabled(LogLevel.Trace)
            End Get
        End Property

        Public ReadOnly Property IsDebugEnabled As Boolean Implements Logger.IsDebugEnabled
            Get
                Return Enabled(LogLevel.Debug)
            End Get
        End Property

        Public ReadOnly Property IsInfoEnabled As Boolean Implements Logger.IsInfoEnabled
            Get
                Return Enabled(LogLevel.Info)
            End Get
        End Property

        Public ReadOnly Property IsWarnEnabled As Boolean Implements Logger.IsWarnEnabled
            Get
                Return Enabled(LogLevel.Warn)
            End Get
        End Property

        Public ReadOnly Property IsErrorEnabled As Boolean Implements Logger.IsErrorEnabled
            Get
                Return Enabled(LogLevel.Error)
            End Get
        End Property

        Protected Overridable Sub Write(ByVal level As LogLevel, ByVal message As String)
            System.Diagnostics.Trace.WriteLine(String.Format("{0:HH:mm:ss.fff} {1,-5} [{2}] {3}", DateTime.Now, level.ToString().ToUpperInvariant(), loggerName, message))
        End Sub

        Private Sub Log(ByVal level As LogLevel, ByVal format As String, ByVal args() As Object)
            If Not Enabled(level) Then Return
            Write(level, MessageFormatter.format(format, args))
        End Sub

        Public Sub trace(ByVal format As String, ByVal ParamArray args() As Object) Implements Logger.trace
            Log(LogLevel.Trace, format, args)
        End Sub

        Public Sub debug(ByVal format As String, ByVal ParamArray args() As Object) Implements Logger.debug
            Log(LogLevel.Debug, format, args)
        End Sub

        Public Sub info(ByVal format As String, ByVal ParamArray args() As Object) Implements Logger.info
            Log(LogLevel.Info, format, args)
        End Sub

        Public Sub warn(ByVal format As String, ByVal ParamArray args() As Object) Implements Logger.warn
            Log(LogLevel.Warn, format, args)
        End Sub

        Public Sub [error](ByVal format As String, ByVal ParamArray args() As Object) Implements Logger.error
            Log(LogLevel.Error, format, args)
        End Sub

        Public Sub [error](ByVal message As String, ByVal exception As Exception) Implements Logger.error
            If Not Enabled(LogLevel.Error) Then Return
            Write(LogLevel.Error, message & If(exception Is Nothing, "", Environment.NewLine & exception.ToString()))
        End Sub
    End Class

End Namespace
