Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports StringQuality = org.datavec.api.transform.quality.columns.StringQuality
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.analysis.quality.string

	Public Class StringQualityAnalysisState
		Implements QualityAnalysisState(Of StringQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.StringQuality stringQuality;
		Private stringQuality As StringQuality
		Private addFunction As StringQualityAddFunction
		Private mergeFunction As StringQualityMergeFunction

		Public Sub New(ByVal stringMetaData As StringMetaData)
			Me.stringQuality = New StringQuality()
			Me.addFunction = New StringQualityAddFunction(stringMetaData)
			Me.mergeFunction = New StringQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As StringQualityAnalysisState
			stringQuality = addFunction.apply(stringQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As StringQualityAnalysisState) As StringQualityAnalysisState
			stringQuality = mergeFunction.apply(stringQuality, other.getStringQuality())
			Return Me
		End Function

		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of StringQualityAnalysisState).getColumnQuality
			Get
				Return stringQuality
			End Get
		End Property
	End Class

End Namespace