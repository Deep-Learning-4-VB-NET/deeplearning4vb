Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports ND4VBIllegalArgumentException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalArgumentException
Imports Condition = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Condition
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports ND4JIllegalArgumentException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalArgumentException
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>choose</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Choose</c>).</summary>
    Public Class Choose
        Inherits DynamicCustomOp

        Private condition As Condition

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal condition As Condition)
			MyBase.New(Nothing, sameDiff, args)
			If condition Is Nothing Then
				Throw New ND4JIllegalArgumentException("Must specify a condition.")
			End If

			Me.inPlace = True
			Me.inplaceCall = True
			addIArgument(condition.condtionNum())
			Me.condition = condition
		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer))
            MyBase.New(opName, inputs, outputs, tArguments, iArguments)
        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal condition As Condition)
            MyBase.New(opName, inputs, Nothing)
            If condition Is Nothing Then
                Throw New ND4VBIllegalArgumentException("Must specify a condition.")
            End If

            addInputArgument(inputs)
            addIArgument(condition.condtionNum())
        End Sub

        ''' 
        ''' <param name="inputs"> </param>
        ''' <param name="condition"> </param>
        Public Sub New(ByVal inputs() As INDArray, ByVal condition As Condition)
            Me.New(inputs, Enumerable.Empty(Of Integer)(),Enumerable.Empty(Of Double)(),condition)
        End Sub

        ''' <summary>
        ''' Note that iArgs (integer arguments) and  tArgs(double/float arguments)
        ''' may end up being used under the following conditions:
        ''' scalar operations (if a scalar is specified the you do not need to specify an ndarray)
        ''' otherwise, if an ndarray is needed as a second input then put it in the inputs
        ''' 
        ''' Usually, you only need 1 input (the equivalent of the array you're trying to do indexing on)
        ''' </summary>
        ''' <param name="inputs"> the inputs in to the op </param>
        ''' <param name="iArgs"> the integer arguments as needed </param>
        ''' <param name="tArgs"> the arguments </param>
        ''' <param name="condition"> the condition to filter on </param>
        Public Sub New(ByVal inputs() As INDArray, ByVal iArgs As IList(Of Integer), ByVal tArgs As IList(Of Double), ByVal condition As Condition)
            MyBase.New(Nothing, inputs, Nothing)
            If condition Is Nothing Then
                Throw New ND4VBIllegalArgumentException("Must specify a condition.")
            End If

            If iArgs.Count > 0 Then
                addIArgument(Ints.toArray(iArgs))
            End If

            If tArgs.Count > 0 Then
                addTArgument(tArgs.ToArray())
            End If
            addIArgument(condition.condtionNum())
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(opName, sameDiff, args, inPlace)
		End Sub
#End If

        Public Sub New()
            'No-arg constructor for use in DifferentialFunctionClassHolder
        End Sub

        Public Overrides Function opName() As String
            Return "choose"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
