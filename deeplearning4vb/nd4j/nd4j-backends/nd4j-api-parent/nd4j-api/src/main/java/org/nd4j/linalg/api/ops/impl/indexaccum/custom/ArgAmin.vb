Imports System.Collections.Generic
Imports Data = lombok.Data
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports BaseDynamicCustomIndexReduction = org.nd4j.linalg.api.ops.impl.reduce.custom.BaseDynamicCustomIndexReduction
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.indexaccum.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ArgAmin extends org.nd4j.linalg.api.ops.impl.reduce.custom.BaseDynamicCustomIndexReduction
	Public Class ArgAmin
		Inherits BaseDynamicCustomIndexReduction

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, args, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal inputs() As INDArray)
			MyBase.New(inputs, Nothing)
		End Sub


		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean)
			MyBase.New(inputs, outputs, keepDims)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(inputs, outputs, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal [dim]() As Integer)
			Me.New(inputs,Nothing,False,[dim])
		End Sub


		Public Overrides Function opName() As String
			Return "argamin"
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

	End Class

End Namespace