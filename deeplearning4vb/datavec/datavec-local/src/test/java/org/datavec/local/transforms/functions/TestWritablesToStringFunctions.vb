Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports SequenceWritablesToStringFunction = org.datavec.local.transforms.misc.SequenceWritablesToStringFunction
Imports WritablesToStringFunction = org.datavec.local.transforms.misc.WritablesToStringFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestWritablesToStringFunctions
	Public Class TestWritablesToStringFunctions
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWritablesToString() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWritablesToString()

			Dim l As IList(Of Writable) = New List(Of Writable) From {
				New DoubleWritable(1.5),
				New Text("someValue")
			}
			Dim expected As String = l(0).ToString() & "," & l(1).ToString()

			assertEquals(expected, (New WritablesToStringFunction(",")).apply(l))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceWritablesToString() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSequenceWritablesToString()

			Dim l As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New DoubleWritable(1.5), New Text("someValue")), Arrays.asList(New DoubleWritable(2.5), New Text("otherValue"))}

			Dim expected As String = l(0)(0).ToString() & "," & l(0)(1).ToString() & vbLf & l(1)(0).ToString() & "," & l(1)(1).ToString()

			assertEquals(expected, (New SequenceWritablesToStringFunction(",")).apply(l))
		End Sub
	End Class

End Namespace