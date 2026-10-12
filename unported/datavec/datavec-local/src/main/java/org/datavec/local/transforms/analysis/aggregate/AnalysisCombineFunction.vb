Imports System.Collections.Generic
Imports org.datavec.api.transform.analysis
Imports org.nd4j.common.function

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

Namespace org.datavec.local.transforms.analysis.aggregate


	Public Class AnalysisCombineFunction
		Implements BiFunction(Of IList(Of AnalysisCounter), IList(Of AnalysisCounter), IList(Of AnalysisCounter))

		Public Overridable Function apply(ByVal l1 As IList(Of AnalysisCounter), ByVal l2 As IList(Of AnalysisCounter)) As IList(Of AnalysisCounter)
			If l1 Is Nothing Then
				Return l2
			End If
			If l2 Is Nothing Then
				Return l1
			End If

			Dim size As Integer = l1.Count
			If size <> l2.Count Then
				Throw New System.InvalidOperationException("List lengths differ")
			End If

			Dim [out] As IList(Of AnalysisCounter) = New List(Of AnalysisCounter)()
			For i As Integer = 0 To size - 1
				[out].Add(l1(i).merge(l2(i)))
			Next i
			Return [out]
		End Function
	End Class

End Namespace