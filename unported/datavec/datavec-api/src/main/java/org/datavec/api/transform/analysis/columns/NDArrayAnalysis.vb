Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType

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

Namespace org.datavec.api.transform.analysis.columns


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Builder(builderClassName = "Builder", builderMethodName = "Builder") @Data public class NDArrayAnalysis implements ColumnAnalysis
	<Serializable>
	Public Class NDArrayAnalysis
		Implements ColumnAnalysis

		Private countTotal As Long
		Private countNull As Long
		Private minLength As Long
		Private maxLength As Long
		Private totalNDArrayValues As Long
		Private countsByRank As IDictionary(Of Integer, Long)
		Private minValue As Double
		Private maxValue As Double
		Protected Friend histogramBuckets() As Double
		Protected Friend histogramBucketCounts() As Long


		Public Overridable ReadOnly Property ColumnType As ColumnType Implements ColumnAnalysis.getColumnType
			Get
				Return ColumnType.NDArray
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim sortedCountsByRank As IDictionary(Of Integer, Long) = New LinkedHashMap(Of Integer, Long)()
			Dim keys As IList(Of Integer) = New List(Of Integer)(If(countsByRank Is Nothing, Enumerable.Empty(Of Integer)(), countsByRank.Keys))
			keys.Sort()
			For Each i As Integer? In keys
				sortedCountsByRank(i) = countsByRank(i)
			Next i

			Return "NDArrayAnalysis(countTotal=" & countTotal & ",countNull=" & countNull & ",minLength=" & minLength & ",maxLength=" & maxLength & ",totalValuesAllNDArrays=" & totalNDArrayValues & ",minValue=" & minValue & ",maxValue=" & maxValue & ",countsByNDArrayRank=" & sortedCountsByRank & ")"
		End Function


	End Class

End Namespace