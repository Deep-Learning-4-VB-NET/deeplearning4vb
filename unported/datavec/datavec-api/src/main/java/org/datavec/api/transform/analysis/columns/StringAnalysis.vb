Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
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
'ORIGINAL LINE: @Data @AllArgsConstructor @NoArgsConstructor public class StringAnalysis implements ColumnAnalysis
	<Serializable>
	Public Class StringAnalysis
		Implements ColumnAnalysis

		Private minLength As Integer
		Private maxLength As Integer
		Private meanLength As Double
		Private sampleStdevLength As Double
		Private sampleVarianceLength As Double
		Private countTotal As Long
		Private histogramBuckets() As Double
		Private histogramBucketCounts() As Long

		Private Sub New(ByVal builder As Builder)
			Me.minLength = builder.minLength_Conflict
			Me.maxLength = builder.maxLength_Conflict
			Me.meanLength = builder.meanLength_Conflict
			Me.sampleStdevLength = builder.sampleStdevLength_Conflict
			Me.sampleVarianceLength = builder.sampleVarianceLength_Conflict
			Me.countTotal = builder.countTotal_Conflict
			Me.histogramBuckets = builder.histogramBuckets_Conflict
			Me.histogramBucketCounts = builder.histogramBucketCounts_Conflict
		End Sub

		Public Overrides Function ToString() As String
			Return "StringAnalysis(minLen=" & minLength & ",maxLen=" & maxLength & ",meanLen=" & meanLength & ",sampleStDevLen=" & sampleStdevLength & ",sampleVarianceLen=" & sampleVarianceLength & ",count=" & countTotal & ")"
		End Function

		Public Overridable ReadOnly Property ColumnType As ColumnType Implements ColumnAnalysis.getColumnType
			Get
				Return ColumnType.String
			End Get
		End Property

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field minLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minLength_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field maxLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maxLength_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field meanLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend meanLength_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field sampleStdevLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sampleStdevLength_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field sampleVarianceLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sampleVarianceLength_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field countTotal was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend countTotal_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field histogramBuckets was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend histogramBuckets_Conflict() As Double
'JAVA TO VB CONVERTER NOTE: The field histogramBucketCounts was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend histogramBucketCounts_Conflict() As Long

'JAVA TO VB CONVERTER NOTE: The parameter minLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minLength(ByVal minLength_Conflict As Integer) As Builder
				Me.minLength_Conflict = minLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter maxLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxLength(ByVal maxLength_Conflict As Integer) As Builder
				Me.maxLength_Conflict = maxLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter meanLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function meanLength(ByVal meanLength_Conflict As Double) As Builder
				Me.meanLength_Conflict = meanLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter sampleStdevLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampleStdevLength(ByVal sampleStdevLength_Conflict As Double) As Builder
				Me.sampleStdevLength_Conflict = sampleStdevLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter sampleVarianceLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampleVarianceLength(ByVal sampleVarianceLength_Conflict As Double) As Builder
				Me.sampleVarianceLength_Conflict = sampleVarianceLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countTotal was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countTotal(ByVal countTotal_Conflict As Long) As Builder
				Me.countTotal_Conflict = countTotal_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter histogramBuckets was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function histogramBuckets(ByVal histogramBuckets_Conflict() As Double) As Builder
				Me.histogramBuckets_Conflict = histogramBuckets_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter histogramBucketCounts was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function histogramBucketCounts(ByVal histogramBucketCounts_Conflict() As Long) As Builder
				Me.histogramBucketCounts_Conflict = histogramBucketCounts_Conflict
				Return Me
			End Function

			Public Overridable Function build() As StringAnalysis
				Return New StringAnalysis(Me)
			End Function
		End Class

	End Class

End Namespace