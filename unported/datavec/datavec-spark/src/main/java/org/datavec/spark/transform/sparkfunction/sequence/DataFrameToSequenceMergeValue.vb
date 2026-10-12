Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Row = org.apache.spark.sql.Row
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports DataFrames = org.datavec.spark.transform.DataFrames

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

Namespace org.datavec.spark.transform.sparkfunction.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DataFrameToSequenceMergeValue implements org.apache.spark.api.java.function.Function2<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, Iterable<org.apache.spark.sql.Row>, java.util.List<java.util.List<org.datavec.api.writable.Writable>>>
	Public Class DataFrameToSequenceMergeValue
		Implements Function2(Of IList(Of IList(Of Writable)), IEnumerable(Of Row), IList(Of IList(Of Writable)))

		Private ReadOnly schema As Schema

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> seqSoFar, Iterable<org.apache.spark.sql.Row> rows) throws Exception
		Public Overrides Function [call](ByVal seqSoFar As IList(Of IList(Of Writable)), ByVal rows As IEnumerable(Of Row)) As IList(Of IList(Of Writable))
			Dim retSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(seqSoFar)
			For Each v1 As Row In rows
				Dim ret As IList(Of Writable) = DataFrames.rowToWritables(schema, v1)
				retSeq.Add(ret)
			Next v1

			retSeq.Sort(New ComparatorAnonymousInnerClass(Me))

			Return retSeq
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of IList(Of Writable))

			Private ReadOnly outerInstance As DataFrameToSequenceMergeValue

			Public Sub New(ByVal outerInstance As DataFrameToSequenceMergeValue)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As IList(Of Writable), ByVal o2 As IList(Of Writable)) As Integer Implements IComparer(Of IList(Of Writable)).Compare
				Return Integer.compare(o1(1).toInt(), o2(1).toInt())
			End Function
		End Class
	End Class

End Namespace