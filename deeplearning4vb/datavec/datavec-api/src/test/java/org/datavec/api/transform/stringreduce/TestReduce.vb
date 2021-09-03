Imports System.Collections.Generic
Imports StringReduceOp = org.datavec.api.transform.StringReduceOp
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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

Namespace org.datavec.api.transform.stringreduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestReduce extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestReduce
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReducerDouble()
		Public Overridable Sub testReducerDouble()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("1"),
				New Text("2")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("1"),
				New Text("2")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("1"),
				New Text("2")
			})

			Dim exp As IDictionary(Of StringReduceOp, String) = New LinkedHashMap(Of StringReduceOp, String)()
			exp(StringReduceOp.MERGE) = "12"
			exp(StringReduceOp.APPEND) = "12"
			exp(StringReduceOp.PREPEND) = "21"
			exp(StringReduceOp.REPLACE) = "2"

			For Each op As StringReduceOp In exp.Keys

				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnString("column").build()

				Dim reducer As StringReducer = (New StringReducer.Builder(op)).build()

				reducer.InputSchema = schema

				Dim [out] As IList(Of Writable) = reducer.reduce(inputs)

				assertEquals(3, [out].Count)
				assertEquals(exp(op), [out](0).ToString())
			Next op
		End Sub


	End Class

End Namespace