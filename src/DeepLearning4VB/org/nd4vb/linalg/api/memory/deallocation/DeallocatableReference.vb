Imports System
Imports Deallocatable = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocatable
Imports Deallocator = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocator

Namespace org.nd4vb.linalg.api.memory.deallocation

    ''' <summary>
    ''' A weak reference to a <see cref="Deallocatable"/> that remembers its id and <see cref="Deallocator"/>, so the
    ''' resources can be released after the referent is collected. Java registers the reference with a
    ''' <c>ReferenceQueue</c>; .NET has no reference queues, so the constructor takes no queue and the owner polls
    ''' <see cref="[get]"/> (or <see cref="WeakReference.IsAlive"/>) instead. The managed backend allocates from the
    ''' garbage-collected heap and needs no deallocator thread.
    ''' </summary>
    Public Class DeallocatableReference
        Inherits WeakReference

        Private id As String
        Private deallocator As Deallocator

        Public Sub New(ByVal referent As Deallocatable)
            MyBase.New(referent)
            Me.id = referent.UniqueId
            Me.deallocator = referent.deallocator()
        End Sub

        ''' <summary>The referent, or Nothing once it has been collected (Java <c>get()</c>).</summary>
        Public Overridable Function [get]() As Deallocatable
            Return TryCast(Target, Deallocatable)
        End Function

        Public Overridable Function getId() As String
            Return id
        End Function

        Public Overridable Sub setId(ByVal id As String)
            Me.id = id
        End Sub

        Public Overridable Function getDeallocator() As Deallocator
            Return deallocator
        End Function

        Public Overridable Sub setDeallocator(ByVal deallocator As Deallocator)
            Me.deallocator = deallocator
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As DeallocatableReference = TryCast(obj, DeallocatableReference)
            If o Is Nothing Then Return False
            Return Object.Equals(id, o.id) AndAlso Object.Equals(deallocator, o.deallocator)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + If(id Is Nothing, 43, id.GetHashCode())
            h = h * 59 + If(deallocator Is Nothing, 43, deallocator.GetHashCode())
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "DeallocatableReference(id=" & If(id, "null") & ", deallocator=" & Objects.toString(deallocator) & ")"
        End Function

    End Class

End Namespace
