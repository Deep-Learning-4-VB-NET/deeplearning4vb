Imports System
Imports IrisUtils = org.deeplearning4j.datasets.base.IrisUtils
Imports BaseDataFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.BaseDataFetcher

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

Namespace org.deeplearning4j.datasets.fetchers



	<Serializable>
	Public Class IrisDataFetcher
		Inherits BaseDataFetcher


		''' 
		Private Const serialVersionUID As Long = 4566329799221375262L
		Public Const NUM_EXAMPLES As Integer = 150

		Public Sub New()
			numOutcomes = 3
			inputColumns_Conflict = 4
			totalExamples_Conflict = NUM_EXAMPLES
		End Sub

		Public Overrides Sub fetch(ByVal numExamples As Integer)
			Dim from As Integer = cursor_Conflict
			Dim [to] As Integer = cursor_Conflict + numExamples
			If [to] > totalExamples_Conflict Then
				[to] = totalExamples_Conflict
			End If

			Try
				initializeCurrFromList(IrisUtils.loadIris(from, [to]))
				cursor_Conflict += numExamples
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to load iris.dat", e)
			End Try
		End Sub


	End Class

End Namespace