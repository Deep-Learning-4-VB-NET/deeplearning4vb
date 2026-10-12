Imports System.Collections.Generic
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports WritablesToNDArrayFunction = org.datavec.local.transforms.misc.WritablesToNDArrayFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.datavec.local.transforms.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class TestWritablesToNDArrayFunction
	Public Class TestWritablesToNDArrayFunction
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritablesToNDArrayAllScalars() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritablesToNDArrayAllScalars()
			Nd4j.DataType = DataType.FLOAT
			Dim l As IList(Of Writable) = New List(Of Writable)()
			For i As Integer = 0 To 4
				l.Add(New IntWritable(i))
			Next i
			Dim expected As INDArray = Nd4j.arange(5f).castTo(DataType.FLOAT).reshape(ChrW(1), 5)
			assertEquals(expected, (New WritablesToNDArrayFunction()).apply(l))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritablesToNDArrayMixed() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritablesToNDArrayMixed()
			Nd4j.DataType = DataType.FLOAT
			Dim l As IList(Of Writable) = New List(Of Writable)()
			l.Add(New IntWritable(0))
			l.Add(New IntWritable(1))
			Dim arr As INDArray = Nd4j.arange(2, 5).reshape(ChrW(1), 3)
			l.Add(New NDArrayWritable(arr))
			l.Add(New IntWritable(5))
			arr = Nd4j.arange(6, 9).reshape(ChrW(1), 3)
			l.Add(New NDArrayWritable(arr))
			l.Add(New IntWritable(9))

			Dim expected As INDArray = Nd4j.arange(10).castTo(DataType.FLOAT).reshape(ChrW(1), 10)
			assertEquals(expected, (New WritablesToNDArrayFunction()).apply(l))
		End Sub
	End Class

End Namespace