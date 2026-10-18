Imports System

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' The description of a custom op as registered with the backend: hash, numbers of inputs, outputs and
    ''' arguments, and whether it can run in place. Lombok's <c>@AllArgsConstructor @Builder @Getter</c> members are
    ''' written out: <c>CustomOpDescriptor.builder().hash(h).numInputs(2).build()</c>.
    ''' </summary>
    Public Class CustomOpDescriptor

        Private hash As Long
        Private numInputs As Integer
        Private numOutputs As Integer
        Private allowsInplace As Boolean
        Private numTArgs As Integer
        Private numIArgs As Integer

        Public Sub New(ByVal hash As Long, ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal allowsInplace As Boolean, ByVal numTArgs As Integer, ByVal numIArgs As Integer)
            Me.hash = hash
            Me.numInputs = numInputs
            Me.numOutputs = numOutputs
            Me.allowsInplace = allowsInplace
            Me.numTArgs = numTArgs
            Me.numIArgs = numIArgs
        End Sub

        Public Shared Function builder() As CustomOpDescriptorBuilder
            Return New CustomOpDescriptorBuilder()
        End Function

        Public Overridable Function getHash() As Long
            Return hash
        End Function

        Public Overridable Function getNumInputs() As Integer
            Return numInputs
        End Function

        Public Overridable Function getNumOutputs() As Integer
            Return numOutputs
        End Function

        Public Overridable Function isAllowsInplace() As Boolean
            Return allowsInplace
        End Function

        Public Overridable Function getNumTArgs() As Integer
            Return numTArgs
        End Function

        Public Overridable Function getNumIArgs() As Integer
            Return numIArgs
        End Function

        ''' <summary>Lombok's builder for <see cref="CustomOpDescriptor"/>.</summary>
        Public Class CustomOpDescriptorBuilder
            Private hashValue As Long
            Private numInputsValue As Integer
            Private numOutputsValue As Integer
            Private allowsInplaceValue As Boolean
            Private numTArgsValue As Integer
            Private numIArgsValue As Integer

            Public Function hash(ByVal value As Long) As CustomOpDescriptorBuilder
                hashValue = value
                Return Me
            End Function

            Public Function numInputs(ByVal value As Integer) As CustomOpDescriptorBuilder
                numInputsValue = value
                Return Me
            End Function

            Public Function numOutputs(ByVal value As Integer) As CustomOpDescriptorBuilder
                numOutputsValue = value
                Return Me
            End Function

            Public Function allowsInplace(ByVal value As Boolean) As CustomOpDescriptorBuilder
                allowsInplaceValue = value
                Return Me
            End Function

            Public Function numTArgs(ByVal value As Integer) As CustomOpDescriptorBuilder
                numTArgsValue = value
                Return Me
            End Function

            Public Function numIArgs(ByVal value As Integer) As CustomOpDescriptorBuilder
                numIArgsValue = value
                Return Me
            End Function

            Public Function build() As CustomOpDescriptor
                Return New CustomOpDescriptor(hashValue, numInputsValue, numOutputsValue, allowsInplaceValue, numTArgsValue, numIArgsValue)
            End Function
        End Class

    End Class

End Namespace
