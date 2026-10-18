Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce.custom

    ''' <summary>
    ''' Base class for reduction.
    ''' 3 main properties matter for any sub class:
    ''' 1. Dimensions can either be an input variable (SDVariable/INDArray) or int arguments.
    ''' 2. If you want the dimensions passed as int arguments, pass in the int dimensions array as a constructor.
    ''' 3. Keep dimensions preserves the rank of the output array relative to the input
    ''' even when doing a reduce.
    ''' 
    ''' @author Adam Gibson
    ''' </summary>
    Public MustInherit Class BaseDynamicCustomReduction
        Inherits DynamicCustomOp

        Protected Friend keepDimsField As Boolean = False
        Protected Friend isComplexField As Boolean = False
        Protected Friend isEmptyReduceField As Boolean = False

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean)
			Me.New(sameDiff,args,keepDims,False)

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			Me.New(sameDiff,args,keepDims,False,dimensions)

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean)
			MyBase.New(Nothing,sameDiff,args)
			Me.isComplex = isComplex
			Me.keepDims = keepDims
			addArgs()

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(Nothing,sameDiff,args)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.dimensions = dimensions
			addArgs()

		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(Nothing,inputs,outputs)

        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean)
            Me.New(inputs,outputs,keepDims,Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            Me.New(inputs,outputs)
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            Me.New(inputs, CType(Nothing, INDArray()), keepDims, dimensions)
        End Sub

        Public Sub New(ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, arg)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.isEmptyReduce = isEmptyReduce
			Me.dimensions = dimensions
			addArgs()

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.isEmptyReduce = isEmptyReduce
			Me.dimensions = dimensions
			addArgs()

		End Sub

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.isEmptyReduce = isEmptyReduce
			Me.dimensions = dimensions
			addArgs()

		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal input As INDArray, ByVal output As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, input, output, tArguments, iArguments)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs, tArguments, iArguments)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer), ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs, tArguments, iArguments)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(inputs, outputs)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName, inputs, outputs)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(opName, sameDiff, args, inPlace)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.isEmptyReduce = isEmptyReduce
			Me.dimensions = dimensions
			addArgs()

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, inPlace)
			Me.keepDims = keepDims
			Me.isComplex = isComplex
			Me.isEmptyReduce = isEmptyReduce
			Me.dimensions = dimensions
			addArgs()

		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal isEmptyReduce As Boolean, ByVal dimensions() As Integer)
            MyBase.New(opName)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.isEmptyReduceField = isEmptyReduce
            Me.dimensionsField = dimensions
            addArgs()

        End Sub

        Public Sub New(ByVal input() As INDArray, ByVal output() As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
            MyBase.New(Nothing,input,output)
            Me.keepDimsField = keepDims
            Me.isComplexField = isComplex
            Me.dimensionsField = dimensions
            addArgs()
        End Sub

        Protected Friend Overridable Sub addArgs()
            addBArgument(keepDimsField)
            If dimensionsField IsNot Nothing Then
                For i As Integer = 0 To dimensionsField.Length - 1
                    addIArgument(dimensionsField(i))
                Next i
            End If

        End Sub

        ''' <summary>
        ''' Calculate the data types for the output arrays.
        ''' Though datatypes can also be inferred from <seealso cref="calculateOutputShape()"/>, this method differs in that it does not
        ''' require the input arrays to be populated.
        ''' This is important as it allows us to do greedy datatype inference for the entire net - even if arrays are not
        ''' available.
        ''' </summary>
        ''' <param name="dataTypes"> The data types of the inputs </param>
        ''' <returns> The data types of the outputs </returns>
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType)) As IList(Of Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType)
            If dArguments.Count > 0 Then
                Return New List(Of Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType) From {dArguments(0)}
            End If
            Return New List(Of Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType) From {dataTypes(0)}
        End Function

        Public Overrides MustOverride Function opName() As String

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>keepDims</c>.</summary>
        Public Overridable Function isKeepDims() As Boolean
            Return keepDimsField
        End Function

        ''' <summary>Lombok setter of <c>keepDims</c>.</summary>
        Public Overridable Sub setKeepDims(ByVal value As Boolean)
            Me.keepDimsField = value
        End Sub

        ''' <summary>Lombok getter of <c>isEmptyReduce</c>.</summary>
        Public Overridable Function isEmptyReduce() As Boolean
            Return isEmptyReduceField
        End Function

        ''' <summary>Lombok setter of <c>isEmptyReduce</c>.</summary>
        Public Overridable Sub setEmptyReduce(ByVal value As Boolean)
            Me.isEmptyReduceField = value
        End Sub

    End Class

End Namespace
