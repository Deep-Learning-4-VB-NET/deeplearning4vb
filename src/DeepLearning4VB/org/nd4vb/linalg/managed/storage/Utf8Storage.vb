' DeepLearning4VB: UTF8 storage of the managed ND4VB engine.

Imports System

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>
    ''' UTF8 storage: strings in a <c>String()</c>. It has no numeric access: the numeric getters, setters
    ''' and fills throw <see cref="NotSupportedException"/> (as ND4J's UTF8 buffers do); use
    ''' <see cref="ManagedStorage.GetString"/> and <see cref="ManagedStorage.SetString"/>.
    ''' </summary>
    Public NotInheritable Class Utf8Storage
        Inherits TypedStorage(Of String)

        ''' <summary>A storage of <paramref name="length"/> elements, all Nothing.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New String(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As String())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.UTF8
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Strings As String()
            Get
                Return _data
            End Get
        End Property

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Throw NoNumericAccess()
        End Function

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            Throw NoNumericAccess()
        End Sub

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Throw NoNumericAccess()
        End Function

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            Throw NoNumericAccess()
        End Sub

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Function GetBoolean(ByVal index As Long) As Boolean
            Throw NoNumericAccess()
        End Function

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Throw NoNumericAccess()
        End Function

        ''' <summary>The string at <paramref name="index"/>.</summary>
        Public Overrides Function GetString(ByVal index As Long) As String
            Return _data(CInt(index))
        End Function

        ''' <summary>Stores a string.</summary>
        Public Overrides Sub SetString(ByVal index As Long, ByVal value As String)
            _data(CInt(index)) = value
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As String()) As TypedStorage(Of String)
            Return New Utf8Storage(data)
        End Function

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As String
            Throw NoNumericAccess()
        End Function

        ''' <summary>Not supported: UTF8 storage has no numeric access.</summary>
        Protected Overrides Function ConvertLong(ByVal value As Long) As String
            Throw NoNumericAccess()
        End Function

        Private Shared Function NoNumericAccess() As System.Exception
            Return New NotSupportedException("UTF8 storage has no numeric access")
        End Function

    End Class

End Namespace
