Imports Getter = lombok.Getter
Imports org.datavec.api.transform.analysis.quality
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports ColumnQuality = org.datavec.api.transform.quality.columns.ColumnQuality
Imports DoubleQuality = org.datavec.api.transform.quality.columns.DoubleQuality
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

Namespace org.datavec.api.transform.analysis.quality.real

	Public Class RealQualityAnalysisState
		Implements QualityAnalysisState(Of RealQualityAnalysisState)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.datavec.api.transform.quality.columns.DoubleQuality realQuality;
		Private realQuality As DoubleQuality
		Private addFunction As RealQualityAddFunction
		Private mergeFunction As RealQualityMergeFunction

		Public Sub New(ByVal realMetaData As DoubleMetaData)
			Me.realQuality = New DoubleQuality()
			Me.addFunction = New RealQualityAddFunction(realMetaData)
			Me.mergeFunction = New RealQualityMergeFunction()
		End Sub

		Public Overridable Function add(ByVal writable As Writable) As RealQualityAnalysisState
			realQuality = addFunction.apply(realQuality, writable)
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As RealQualityAnalysisState) As RealQualityAnalysisState
			realQuality = mergeFunction.apply(realQuality, other.getRealQuality())
			Return Me
		End Function

		Public Overridable ReadOnly Property ColumnQuality As ColumnQuality Implements QualityAnalysisState(Of RealQualityAnalysisState).getColumnQuality
			Get
				Return realQuality
			End Get
		End Property
	End Class

End Namespace