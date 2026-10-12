Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports RowVectorDeserializer = org.nd4j.linalg.lossfunctions.serde.RowVectorDeserializer
Imports RowVectorSerializer = org.nd4j.linalg.lossfunctions.serde.RowVectorSerializer
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.serde

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.JACKSON_SERDE) public class JsonSerdeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class JsonSerdeTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNDArrayTextSerializer(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNDArrayTextSerializer(ByVal backend As Nd4jBackend)
			For Each order As Char In New Char(){"c"c, "f"c}
				Nd4j.factory().Order = order
				For Each globalDT As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					Nd4j.setDefaultDataTypes(globalDT, globalDT)

					Nd4j.Random.setSeed(12345)
					Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 4).muli(20).subi(10)

					Dim om As val = New ObjectMapper()

					For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UBYTE, DataType.BOOL, DataType.UTF8}

						Dim arr As INDArray
						If dt = DataType.UTF8 Then
							arr = Nd4j.create("aaaaa", "bbbb", "ccc", "dd", "e", "f", "g", "h", "i", "j", "k", "l").reshape("c"c, 3, 4)
						Else
							arr = [in].castTo(dt)
						End If

						Dim tc As New TestClass(arr)

						Dim s As String = om.writeValueAsString(tc)
	'                    System.out.println(dt);
	'                    System.out.println(s);
	'                    System.out.println("\n\n\n");

						Dim deserialized As TestClass = om.readValue(s, GetType(TestClass))
						assertEquals(tc, deserialized,dt.ToString())
					Next dt
				Next globalDT
			Next order
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBackwardCompatability(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBackwardCompatability(ByVal backend As Nd4jBackend)
			Nd4j.NDArrayFactory.Order = "f"c

			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(dt, dt)
				'NDArrayTextDeserializer will be used in ILossFunction instances that used to use RowVectorSerializer - and it needs to support old format

				Dim arr As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, 5})
				Dim r As New TestClassRow(arr)

				Dim om As New ObjectMapper()
				Dim s As String = om.writeValueAsString(r)

				Dim tc As TestClass = om.readValue(s, GetType(TestClass))

				assertEquals(arr, tc.getArr())

			Next dt

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @Data @NoArgsConstructor public static class TestClass
		Public Class TestClass
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) public org.nd4j.linalg.api.ndarray.INDArray arr;
			Public arr As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestClass(@JsonProperty("arr") org.nd4j.linalg.api.ndarray.INDArray arr)
			Public Sub New(ByVal arr As INDArray)
				Me.arr = arr
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @Data @NoArgsConstructor public static class TestClassRow
		Public Class TestClassRow
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonDeserialize(using = org.nd4j.linalg.lossfunctions.serde.RowVectorDeserializer.class) @JsonSerialize(using = org.nd4j.linalg.lossfunctions.serde.RowVectorSerializer.class) public org.nd4j.linalg.api.ndarray.INDArray arr;
			Public arr As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestClassRow(@JsonProperty("arr") org.nd4j.linalg.api.ndarray.INDArray arr)
			Public Sub New(ByVal arr As INDArray)
				Me.arr = arr
			End Sub
		End Class

	End Class

End Namespace