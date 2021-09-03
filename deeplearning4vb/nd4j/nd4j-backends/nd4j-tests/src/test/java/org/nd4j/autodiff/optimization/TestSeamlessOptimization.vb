Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports Identity = org.nd4j.linalg.api.ops.impl.transforms.same.Identity
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.Assert.assertEquals
import static org.junit.Assert.fail

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
Namespace org.nd4j.autodiff.optimization


	Public Class TestSeamlessOptimization
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path tempDir;
		Friend tempDir As Path


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOutput(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testOutput(ByVal nd4jBackend As Nd4jBackend)

			'Ensure that optimizer is actually used when calling output methods:
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 3))

			Dim i1 As SDVariable = sd.identity([in])
			Dim i2 As SDVariable = sd.identity(w)
			Dim i3 As SDVariable = sd.identity(b)

			Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", sd.identity(i1.mmul(i2).add(i3)))

			Dim l As New RecordOpsListener()
			sd.setListeners(New AssertNoOpsOfTypeListener(GetType(Identity)), l)

			Dim ph As IDictionary(Of String, INDArray) = Collections.singletonMap("in", Nd4j.rand(DataType.FLOAT, 10, 4))

			For i As Integer = 0 To 2
				l.ops.Clear()

				Select Case i
					Case 0
						sd.outputSingle(ph, "out")
					Case 1
						sd.output(ph, "out")
					Case 2
						sd.batchOutput().output("out").input("in", ph("in")).outputSingle()
				End Select


				Dim expClasses As IList(Of Type) = New List(Of Type) From {GetType(Mmul), GetType(AddOp), GetType(SoftMax)}
				assertEquals(3, l.ops.Count)
				For j As Integer = 0 To 2
					assertEquals(expClasses(j), l.ops(j).getOp().GetType())
				Next j

			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDifferentOutputs(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testDifferentOutputs(ByVal nd4jBackend As Nd4jBackend)
			'Test when the user requests different outputs instead
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGraphModification(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testGraphModification(ByVal nd4jBackend As Nd4jBackend)
			'User modifies the graph -> should reoptimize?

			fail("Not yet implemented")
		End Sub

		Public Class AssertNoOpsOfTypeListener
			Inherits BaseListener

			Friend list As IList(Of Type)

			Public Sub New(ParamArray ByVal c() As Type)
				Preconditions.checkState(c IsNot Nothing AndAlso c.Length > 0, "No classes provided")
				Me.list = New List(Of Type) From {c}
			End Sub

			Public Overrides Function isActive(ByVal operation As Operation) As Boolean
				Return True
			End Function

			Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
				If list.Contains(op.Op.GetType()) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.InvalidOperationException("Encountered unexpected class: " & op.Op.GetType().FullName)
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class RecordOpsListener extends org.nd4j.autodiff.listeners.BaseListener
		Public Class RecordOpsListener
			Inherits BaseListener

			Friend ops As IList(Of SameDiffOp) = New List(Of SameDiffOp)()

			Public Overrides Function isActive(ByVal operation As Operation) As Boolean
				Return True
			End Function

			Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
				ops.Add(op)

			End Sub
		End Class


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class
End Namespace