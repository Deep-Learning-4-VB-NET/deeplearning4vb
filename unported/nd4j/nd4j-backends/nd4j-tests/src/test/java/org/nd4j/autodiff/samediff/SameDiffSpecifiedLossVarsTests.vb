Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports SingletonMultiDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonMultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.autodiff.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @Tag(TagNames.TRAINING) @Tag(TagNames.LOSS_FUNCTIONS) public class SameDiffSpecifiedLossVarsTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SameDiffSpecifiedLossVarsTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecifiedLoss1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecifiedLoss1(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.var("ph", DataType.FLOAT, 3, 4)
			ph1.Array = Nd4j.create(DataType.FLOAT, 3, 4)

			Dim add As SDVariable = ph1.add(1)

			Dim shape As SDVariable = add.shape()
			Dim [out] As SDVariable = add.sum("sum")

			sd.setLossVariables("sum")
			sd.createGradFunction()

			assertFalse(shape.hasGradient())
			Try
				assertNull(shape.gradient())
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("only floating point variables"))
			End Try
			assertNotNull([out].gradient())
			assertNotNull(add.gradient())
			assertNotNull(ph1.gradient())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecifiedLoss2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecifiedLoss2(ByVal backend As Nd4jBackend)
			For i As Integer = 0 To 1
				Dim sd As SameDiff = SameDiff.create()
				Dim ph As SDVariable = sd.placeHolder("ph", DataType.FLOAT, 3, 4)
				Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 5))
				Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 5))

				Dim mmul As SDVariable = ph.mmul(w)
				Dim badd As SDVariable = mmul.add(b)

				Dim add As SDVariable = badd.add(1)

				Dim shape As SDVariable = add.shape()
				Dim unused1 As SDVariable = ph.mul(2)
				Dim unused2 As SDVariable = ph.sub(4)
				Dim unused3 As SDVariable = unused1.div(unused2)
				Dim loss1 As SDVariable = add.std("l1", True)
				Dim loss2 As SDVariable = mmul.mean("l2")

	'            System.out.println(sd.summary());
				sd.summary()

				If i = 0 Then
					sd.setLossVariables("l1", "l2")
					sd.createGradFunction()
				Else
					Dim tc As TrainingConfig = TrainingConfig.builder().updater(New Adam(0.01)).minimize("l1","l2").dataSetFeatureMapping("ph").markLabelsUnused().build()
					sd.TrainingConfig = tc
					Dim ds As New DataSet(Nd4j.create(3,4), Nothing)
					sd.fit(ds)
					sd.fit(ds)
				End If

				For Each s As String In New String(){"w", "b", badd.name(), add.name(), "l1", "l2"}
					Dim gradVar As SDVariable = sd.getVariable(s).gradient()
					assertNotNull(gradVar,s)
				Next s
				'Unused:
				assertFalse(shape.hasGradient())
				Try
					assertNull(shape.gradient())
				Catch e As System.InvalidOperationException
					assertTrue(e.Message.contains("only floating point variables"))
				End Try
				For Each s As String In New String(){unused1.name(), unused2.name(), unused3.name()}
					assertNull(sd.getVariable(s).gradient())
				Next s
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrainingDifferentLosses(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrainingDifferentLosses(ByVal backend As Nd4jBackend)
			'Net with 2 losses: train on the first one, then change losses
			'Also check that if modifying via add/setLossVariables the training config changes

			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.placeHolder("ph1", DataType.FLOAT, 3, 4)
			Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(DataType.FLOAT, 4, 5))
			Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(DataType.FLOAT, 5))

			Dim mmul1 As SDVariable = ph1.mmul(w1)
			Dim badd1 As SDVariable = mmul1.add(b1)


			Dim ph2 As SDVariable = sd.placeHolder("ph2", DataType.FLOAT, 3, 2)
			Dim w2 As SDVariable = sd.var("w2", Nd4j.rand(DataType.FLOAT, 2, 6))
			Dim b2 As SDVariable = sd.var("b2", Nd4j.rand(DataType.FLOAT, 6))

			Dim mmul2 As SDVariable = ph2.mmul(w2)
			Dim badd2 As SDVariable = mmul2.add(b2)

			Dim loss1 As SDVariable = badd1.std("loss1",True)
			Dim loss2 As SDVariable = badd2.std("loss2", True)


			'First: create grad function for optimizing loss 1 only
			sd.setLossVariables("loss1")
			sd.createGradFunction()
			For Each v As SDVariable In New SDVariable(){ph1, w1, b1, mmul1, badd1, loss1}
				assertNotNull(v.gradient(),v.name())
			Next v
			For Each v As SDVariable In New SDVariable(){ph2, w2, b2, mmul2, badd2, loss2}
				assertNull(v.gradient(),v.name())
			Next v

			'Now, set to other loss function
			sd.setLossVariables("loss2")
			sd.createGradFunction()
			For Each v As SDVariable In New SDVariable(){ph1, w1, b1, mmul1, badd1, loss1}
				assertNull(v.gradient(),v.name())
			Next v
			For Each v As SDVariable In New SDVariable(){ph2, w2, b2, mmul2, badd2, loss2}
				assertNotNull(v.gradient(),v.name())
			Next v

			'Train the first side of the graph. The other side should remain unmodified!
			sd.setLossVariables("loss1")
			Dim w1Before As INDArray = w1.Arr.dup()
			Dim b1Before As INDArray = b1.Arr.dup()
			Dim w2Before As INDArray = w2.Arr.dup()
			Dim b2Before As INDArray = b2.Arr.dup()


			Dim tc As TrainingConfig = TrainingConfig.builder().updater(New Adam(1e-2)).dataSetFeatureMapping("ph1","ph2").markLabelsUnused().build()
			sd.TrainingConfig = tc

			Dim mds As New MultiDataSet(New INDArray(){Nd4j.rand(DataType.FLOAT, 3,4), Nd4j.rand(DataType.FLOAT, 3,2)}, New INDArray(){})

			sd.fit(New SingletonMultiDataSetIterator(mds), 3)
			assertNotEquals(w1Before, w1.Arr)
			assertNotEquals(b1Before, b1.Arr)
			assertEquals(w2Before, w2.Arr)
			assertEquals(b2Before, b2.Arr)

			'Train second side of graph; first side should be unmodified
			sd.setLossVariables("loss2")
			w1Before = w1.Arr.dup()
			b1Before = b1.Arr.dup()
			w2Before = w2.Arr.dup()
			b2Before = b2.Arr.dup()

			sd.fit(New SingletonMultiDataSetIterator(mds), 3)
			assertEquals(w1Before, w1.Arr)
			assertEquals(b1Before, b1.Arr)
			assertNotEquals(w2Before, w2.Arr)
			assertNotEquals(b2Before, b2.Arr)

		End Sub
	End Class

End Namespace