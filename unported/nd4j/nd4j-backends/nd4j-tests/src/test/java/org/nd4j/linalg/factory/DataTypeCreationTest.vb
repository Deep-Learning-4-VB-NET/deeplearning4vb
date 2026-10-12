Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.factory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.RNG) @NativeTag public class DataTypeCreationTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DataTypeCreationTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataTypeCreation()
		Public Overridable Sub testDataTypeCreation()
			For Each dataType As DataType In New DataType() {DataType.DOUBLE, DataType.FLOAT}
				assertEquals(dataType,Nd4j.create(dataType,1,2).dataType())
				assertEquals(dataType,Nd4j.rand(dataType,1,2).dataType())
				assertEquals(dataType,Nd4j.randn(dataType,1,2).dataType())
				assertEquals(dataType,Nd4j.rand(dataType,"c"c,1,2).dataType())
				assertEquals(dataType,Nd4j.randn(dataType,"c"c,1,2).dataType())

			Next dataType
		End Sub

	End Class

End Namespace