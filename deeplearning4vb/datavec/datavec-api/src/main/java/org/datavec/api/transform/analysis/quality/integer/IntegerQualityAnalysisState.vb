Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports IntegerQuality = org.datavec.api.transform.quality.columns.IntegerQuality
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

Namespace org.datavec.api.transform.analysis.quality.integer

	Public Class IntegerQualityAnalysisState
		Implements QualityAnalysisState(Of IntegerQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.IntegerQuality integerQuality;
		Private integerQuality As IntegerQuality
		Private addFunction As IntegerQualityAddFunction
		Private mergeFunction As IntegerQualityMergeFunction

		Public Sub New(ByVal integerMetaData As IntegerMetaData)
			Me.integerQuality = New IntegerQuality(0, 0, 0, 0, 0)
			Me.addFunction = New IntegerQualityAddFunction(integerMetaData)
			Me.mergeFunction = New IntegerQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As IntegerQualityAnalysisState
			integerQuality = addFunction.apply(integerQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As IntegerQualityAnalysisState) As IntegerQualityAnalysisState
			integerQuality = mergeFunction.apply(integerQuality, other.getIntegerQuality())
			Return Me
		End Function

		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of IntegerQualityAnalysisState).getColumnQuality
			Get
				Return integerQuality
			End Get
		End Property
	End Class

End Namespace