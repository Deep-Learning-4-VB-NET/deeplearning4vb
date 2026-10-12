Imports System
Imports HistogramBin = org.deeplearning4j.ui.model.weights.HistogramBin
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.deeplearning4j.ui.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class HistogramBinTest
	Public Class HistogramBinTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetBins() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGetBins()
			Dim array As INDArray = Nd4j.create(New Double() {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.0})

			Dim histogram As HistogramBin = (New HistogramBin.Builder(array)).setBinCount(10).build()

			assertEquals(0.1, histogram.getMin(), 0.001)
			assertEquals(1.0, histogram.getMax(), 0.001)

			Console.WriteLine("Result: " & histogram.getBins())

			assertEquals(2, histogram.getBins().getDouble(9), 0.001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetData1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGetData1()
			Dim array As INDArray = Nd4j.create(New Double() {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.0})

			Dim histogram As HistogramBin = (New HistogramBin.Builder(array)).setBinCount(10).build()

			assertEquals(0.1, histogram.getMin(), 0.001)
			assertEquals(1.0, histogram.getMax(), 0.001)

			Console.WriteLine("Result: " & histogram.getData())

			assertEquals(10, histogram.getData().size())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetData2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGetData2()
			Dim array As INDArray = Nd4j.create(New Double() {-1.0f, -0.50f, 0.0f, 0.50f, 1.0f, -1.0f, -0.50f, 0.0f, 0.50f, 1.0f})

			Dim histogram As HistogramBin = (New HistogramBin.Builder(array)).setBinCount(10).build()

			assertEquals(-1.0, histogram.getMin(), 0.001)
			assertEquals(1.0, histogram.getMax(), 0.001)

			Console.WriteLine("Result: " & histogram.getData())

			assertEquals(10, histogram.getData().size())

			assertEquals(2, histogram.getData().get(New Decimal("1.00")).get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetData4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGetData4()
			Dim array As INDArray = Nd4j.create(New Double() {-1.0f, -0.50f, 0.0f, 0.50f, 1.0f, -1.0f, -0.50f, 0.0f, 0.50f, 1.0f})

			Dim histogram As HistogramBin = (New HistogramBin.Builder(array)).setBinCount(50).build()

			assertEquals(-1.0, histogram.getMin(), 0.001)
			assertEquals(1.0, histogram.getMax(), 0.001)

			Console.WriteLine("Result: " & histogram.getData())

			assertEquals(50, histogram.getData().size())

			assertEquals(2, histogram.getData().get(New Decimal("1.00")).get())
		End Sub
	End Class

End Namespace