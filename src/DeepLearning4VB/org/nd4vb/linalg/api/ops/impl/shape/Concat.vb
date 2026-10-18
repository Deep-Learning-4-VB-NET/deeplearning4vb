Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports ConcatBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.shape.bp.ConcatBp
#End If
#If MODEL_IMPORT Then
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>concat</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.Concat</c>).</summary>
    Public Class Concat
        Inherits DynamicCustomOp

        Private concatDimension As Integer = -1
        Private isDynamicAxis As Boolean = False

        Public Sub New()

        End Sub

        Public Sub New(ByVal concatDimension As Integer, ParamArray ByVal arrays() As INDArray)
            MyBase.New(Nothing, arrays, Nothing)
            Me.concatDimension = concatDimension
            addIArgument(concatDimension)
        End Sub

        Public Sub New(ByVal arrays() As INDArray, ByVal concatDimension As Integer)
            Me.New(concatDimension, arrays)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal concatDimension As Integer)
			Me.New(sameDiff, concatDimension, inputs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal concatDimension As Integer, ParamArray ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputs)
			addIArgument(concatDimension)
			Me.concatDimension = concatDimension
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "concat"
        End Function

        Public Overrides Sub assertValidForExecution()
            Dim descriptor = Me.Descriptor
            If descriptor Is Nothing Then
                Throw New NoOpNameFoundException("No descriptor found for op name " & opName())
            End If

            If descriptor.getNumInputs() > 0 AndAlso numInputArguments() < 2 Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numInputArguments() & " but should be " & descriptor.getNumInputs())
            End If

            If descriptor.getNumOutputs() > 0 AndAlso numOutputArguments() <> descriptor.getNumOutputs() Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of outputs is invalid for execution. Specified " & numOutputArguments() & " but should be " & descriptor.getNumOutputs())
            End If

            '< 0 means dynamic size
            If descriptor.getNumIArgs() >= 0 AndAlso numIArguments() <> descriptor.getNumIArgs() Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of integer arguments is invalid for execution. Specified " & numIArguments() & " but should be " & descriptor.getNumIArgs())
            End If

            If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() <> descriptor.getNumTArgs() Then
                Throw New ND4VBIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numTArguments() & " but should be " & descriptor.getNumTArgs())
            End If

        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'TF uses dynamic axis - last argument is a scalar integer array for axis
			addBArgument(True)
			isDynamicAxis = True
		End Sub
#End If

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("concatDimension") = concatDimension
            Return ret
        End Function

        Public Overrides Function onnxName() As String
            Return "Concat"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Concat"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String() {"Concat", "ConcatV2"}
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim args() As SDVariable = Me.args()
			Dim bpArgs() As SDVariable
			If isDynamicAxis Then
				bpArgs = Arrays.CopyOf(args, args.Length + 2)
				bpArgs(bpArgs.Length - 1) = bpArgs(bpArgs.Length - 3) 'Last input is axis -> move to end of bp args too
				bpArgs(bpArgs.Length - 2) = i_v(0)
				Return New List(Of SDVariable) From {(New ConcatBp(sameDiff, concatDimension, bpArgs)).outputVariables()}
			Else
				bpArgs = Arrays.CopyOf(args, args.Length + 1)
				bpArgs(bpArgs.Length - 1) = i_v(0)
				Return New List(Of SDVariable) From {(New ConcatBp(sameDiff, concatDimension, bpArgs)).outputVariables()}
			End If
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            If dArguments.Count > 0 Then
                Return New List(Of DataType) From {dArguments(0)}
            End If

            Dim first As DataType = dataTypes(0)

            For i As Integer = 1 To (dataTypes.Count - (If(isDynamicAxis, 1, 0))) - 1
                Dim dt As DataType = dataTypes(i)
                Preconditions.checkState(first = dt, "All inputs must have same datatype - got %s and %s for inputs 0 and %s respectively", first, dt, i)
            Next i

            If isDynamicAxis Then
                Preconditions.checkState(dataTypes(dataTypes.Count - 1).isIntType(), "For dynamic axis case, last datatype must be an integer type, got input types %s")
            End If

            'Output type is same as input types
            Return New List(Of DataType) From {first}
        End Function

    End Class

End Namespace
