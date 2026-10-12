Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports CategoricalQuality = org.datavec.api.transform.quality.columns.CategoricalQuality
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
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

Namespace org.datavec.api.transform.analysis.quality.categorical

	Public Class CategoricalQualityAnalysisState
		Implements QualityAnalysisState(Of CategoricalQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.CategoricalQuality categoricalQuality;
		Private categoricalQuality As CategoricalQuality
		Private addFunction As CategoricalQualityAddFunction
		Private mergeFunction As CategoricalQualityMergeFunction

		Public Sub New(ByVal integerMetaData As CategoricalMetaData)
			Me.categoricalQuality = New CategoricalQuality()
			Me.addFunction = New CategoricalQualityAddFunction(integerMetaData)
			Me.mergeFunction = New CategoricalQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As CategoricalQualityAnalysisState
			categoricalQuality = addFunction.apply(categoricalQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As CategoricalQualityAnalysisState) As CategoricalQualityAnalysisState
			categoricalQuality = mergeFunction.apply(categoricalQuality, other.getCategoricalQuality())
			Return Me
		End Function


		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of CategoricalQualityAnalysisState).getColumnQuality
			Get
				Return categoricalQuality
			End Get
		End Property
	End Class

End Namespace