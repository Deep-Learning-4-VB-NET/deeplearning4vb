Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports LongMetaData = org.datavec.api.transform.metadata.LongMetaData
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports LongQuality = org.datavec.api.transform.quality.columns.LongQuality
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

Namespace org.datavec.api.transform.analysis.quality.longq

	Public Class LongQualityAnalysisState
		Implements QualityAnalysisState(Of LongQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.LongQuality longQuality;
		Private longQuality As LongQuality
		Private addFunction As LongQualityAddFunction
		Private mergeFunction As LongQualityMergeFunction

		Public Sub New(ByVal longMetaData As LongMetaData)
			Me.longQuality = New LongQuality()
			Me.addFunction = New LongQualityAddFunction(longMetaData)
			Me.mergeFunction = New LongQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As LongQualityAnalysisState
			longQuality = addFunction.apply(longQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As LongQualityAnalysisState) As LongQualityAnalysisState
			longQuality = mergeFunction.apply(longQuality, other.getLongQuality())
			Return Me
		End Function

		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of LongQualityAnalysisState).getColumnQuality
			Get
				Return longQuality
			End Get
		End Property
	End Class

End Namespace