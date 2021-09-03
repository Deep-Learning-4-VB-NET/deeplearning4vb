Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports TimeQuality = org.datavec.api.transform.quality.columns.TimeQuality
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

Namespace org.datavec.api.transform.analysis.quality.time

	Public Class TimeQualityAnalysisState
		Implements QualityAnalysisState(Of TimeQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.TimeQuality timeQuality;
		Private timeQuality As TimeQuality
		Private addFunction As TimeQualityAddFunction
		Private mergeFunction As TimeQualityMergeFunction

		Public Sub New(ByVal timeMetaData As TimeMetaData)
			Me.timeQuality = New TimeQuality()
			Me.addFunction = New TimeQualityAddFunction(timeMetaData)
			Me.mergeFunction = New TimeQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As TimeQualityAnalysisState
			timeQuality = addFunction.apply(timeQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As TimeQualityAnalysisState) As TimeQualityAnalysisState
			timeQuality = mergeFunction.apply(timeQuality, other.getTimeQuality())
			Return Me
		End Function

		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of TimeQualityAnalysisState).getColumnQuality
			Get
				Return timeQuality
			End Get
		End Property
	End Class

End Namespace