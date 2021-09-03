Imports System.Collections.Generic
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NDArrayToWritablesFunction = org.datavec.spark.transform.misc.NDArrayToWritablesFunction
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

Namespace org.datavec.spark.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestNDArrayToWritablesFunction
	Public Class TestNDArrayToWritablesFunction
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayToWritablesScalars() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNDArrayToWritablesScalars()
			Dim arr As INDArray = Nd4j.arange(5)
			Dim expected As IList(Of Writable) = New List(Of Writable)()
			For i As Integer = 0 To 4
				expected.Add(New DoubleWritable(i))
			Next i
			Dim actual As IList(Of Writable) = (New NDArrayToWritablesFunction()).call(arr)
			assertEquals(expected, actual)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayToWritablesArray() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNDArrayToWritablesArray()
			Dim arr As INDArray = Nd4j.arange(5)
			Dim expected As IList(Of Writable) = New List(Of Writable) From {New NDArrayWritable(arr)}
			Dim actual As IList(Of Writable) = (New NDArrayToWritablesFunction(True)).call(arr)
			assertEquals(expected, actual)
		End Sub
	End Class

End Namespace