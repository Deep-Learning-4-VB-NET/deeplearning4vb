Imports Tensor = org.apache.arrow.flatbuf.Tensor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.arrow

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class ArrowSerdeTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class ArrowSerdeTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBackAndForth()
		Public Overridable Sub testBackAndForth()
			Dim arr As INDArray = Nd4j.linspace(1,4,4)
			Dim tensor As Tensor = ArrowSerde.toTensor(arr)
			Dim arr2 As INDArray = ArrowSerde.fromTensor(tensor)
			assertEquals(arr,arr2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerializeView()
		Public Overridable Sub testSerializeView()
			Dim matrix As INDArray = Nd4j.linspace(1,8,8).reshape(ChrW(2), 4)
			Dim tensor As Tensor = ArrowSerde.toTensor(matrix.slice(0))
			Dim from As INDArray = ArrowSerde.fromTensor(tensor)
			assertEquals(matrix.data().dataType(),from.data().dataType())
			assertEquals(matrix.slice(0),from)
		End Sub

	End Class

End Namespace