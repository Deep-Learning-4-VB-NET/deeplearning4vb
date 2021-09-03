Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MovingWindowMatrix = org.deeplearning4j.core.util.MovingWindowMatrix
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Moving Window Matrix Test") @NativeTag @Tag(TagNames.NDARRAY_ETL) class MovingWindowMatrixTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MovingWindowMatrixTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Moving Window") void testMovingWindow()
		Friend Overridable Sub testMovingWindow()
			Dim ones As INDArray = Nd4j.ones(4, 4)
			Dim m As New MovingWindowMatrix(ones, 2, 2)
			Dim windows As IList(Of INDArray) = m.windows()
			assertEquals(4, windows.Count)
			Dim m2 As New MovingWindowMatrix(ones, 2, 2, True)
			Dim windowsRotate As IList(Of INDArray) = m2.windows()
			assertEquals(16, windowsRotate.Count)
		End Sub
	End Class

End Namespace