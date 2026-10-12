Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Dataset = org.apache.spark.sql.Dataset
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


	Public Class DataFrameToSequenceMergeCombiner
		Implements Function2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> v1, java.util.List<java.util.List<org.datavec.api.writable.Writable>> v2) throws Exception
		Public Overrides Function [call](ByVal v1 As IList(Of IList(Of Writable)), ByVal v2 As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(v1.Count + v2.Count)
			CType([out], List(Of IList(Of Writable))).AddRange(v1)
			CType([out], List(Of IList(Of Writable))).AddRange(v2)
			[out].Sort(New ComparatorAnonymousInnerClass(Me))
			Return [out]
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of IList(Of Writable))

			Private ReadOnly outerInstance As DataFrameToSequenceMergeCombiner

			Public Sub New(ByVal outerInstance As DataFrameToSequenceMergeCombiner)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As IList(Of Writable), ByVal o2 As IList(Of Writable)) As Integer Implements IComparer(Of IList(Of Writable)).Compare
				Return Integer.compare(o1(1).toInt(), o2(1).toInt())
			End Function
		End Class
	End Class

End Namespace