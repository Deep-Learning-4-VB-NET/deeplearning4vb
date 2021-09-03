Imports System
Imports System.Text
Imports TDigest = com.tdunning.math.stats.TDigest
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports TDigestDeserializer = org.datavec.api.transform.analysis.json.TDigestDeserializer
Imports TDigestSerializer = org.datavec.api.transform.analysis.json.TDigestSerializer
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"digest"}) public abstract class NumericalColumnAnalysis implements ColumnAnalysis
	<Serializable>
	Public MustInherit Class NumericalColumnAnalysis
		Implements ColumnAnalysis

		Public MustOverride ReadOnly Property ColumnType As org.datavec.api.transform.ColumnType Implements ColumnAnalysis.getColumnType
		Public MustOverride ReadOnly Property CountTotal As Long Implements ColumnAnalysis.getCountTotal

		Protected Friend mean As Double
		Protected Friend sampleStdev As Double
		Protected Friend sampleVariance As Double
		Protected Friend countZero As Long
		Protected Friend countNegative As Long
		Protected Friend countPositive As Long
		Protected Friend countMinValue As Long
		Protected Friend countMaxValue As Long
		Protected Friend countTotal As Long
		Protected Friend histogramBuckets() As Double
		Protected Friend histogramBucketCounts() As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.datavec.api.transform.analysis.json.TDigestSerializer.class) @JsonDeserialize(using = org.datavec.api.transform.analysis.json.TDigestDeserializer.class) protected com.tdunning.math.stats.TDigest digest;
		Protected Friend digest As TDigest

		Protected Friend Sub New(ByVal builder As Builder)
			Me.mean = builder.mean
			Me.sampleStdev = builder.sampleStdev
			Me.sampleVariance = builder.sampleVariance
			Me.countZero = builder.countZero
			Me.countNegative = builder.countNegative
			Me.countPositive = builder.countPositive
			Me.countMinValue = builder.countMinValue
			Me.countMaxValue = builder.countMaxValue
			Me.countTotal = builder.countTotal
			Me.histogramBuckets = builder.histogramBuckets
			Me.histogramBucketCounts = builder.histogramBucketCounts
			Me.digest = builder.digest
		End Sub

		Protected Friend Sub New()
			'No arg for Jackson
		End Sub

		Public Overrides Function ToString() As String
			Dim q As String = ""
			If digest IsNot Nothing Then
				Dim quantiles As New StringBuilder(", quantiles=[")
				Dim printReports() As Double = {0.001, 0.01, 0.1, 0.5, 0.9, 0.99, 0.999}
				For i As Integer = 0 To printReports.Length - 1
					quantiles.Append(printReports(i)).Append(" -> ").Append(digest.quantile(printReports(i)))
					If i < printReports.Length - 1 Then
						quantiles.Append(",")
					End If
				Next i
				quantiles.Append("]")
				q = quantiles.ToString()
			End If
			Return "mean=" & mean & ",sampleStDev=" & sampleStdev & ",sampleVariance=" & sampleVariance & ",countZero=" & countZero & ",countNegative=" & countNegative & ",countPositive=" & countPositive & ",countMinValue=" & countMinValue & ",countMaxValue=" & countMaxValue & ",count=" & countTotal & q
		End Function

		Public MustOverride ReadOnly Property MinDouble As Double

		Public MustOverride ReadOnly Property MaxDouble As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public abstract static class Builder<T extends Builder<T>>
		Public MustInherit Class Builder(Of T As Builder(Of T))
'JAVA TO VB CONVERTER NOTE: The field mean was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend mean_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field sampleStdev was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sampleStdev_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field sampleVariance was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sampleVariance_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field countZero was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countZero_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countNegative was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countNegative_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countPositive was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countPositive_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countMinValue was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countMinValue_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countMaxValue was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countMaxValue_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countTotal was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend countTotal_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field histogramBuckets was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend histogramBuckets_Conflict() As Double
'JAVA TO VB CONVERTER NOTE: The field histogramBucketCounts was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend histogramBucketCounts_Conflict() As Long
'JAVA TO VB CONVERTER NOTE: The field digest was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend digest_Conflict As TDigest

'JAVA TO VB CONVERTER NOTE: The parameter mean was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function mean(ByVal mean_Conflict As Double) As T
				Me.mean_Conflict = mean_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter sampleStdev was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampleStdev(ByVal sampleStdev_Conflict As Double) As T
				Me.sampleStdev_Conflict = sampleStdev_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter sampleVariance was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampleVariance(ByVal sampleVariance_Conflict As Double) As T
				Me.sampleVariance_Conflict = sampleVariance_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countZero was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countZero(ByVal countZero_Conflict As Long) As T
				Me.countZero_Conflict = countZero_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countNegative was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countNegative(ByVal countNegative_Conflict As Long) As T
				Me.countNegative_Conflict = countNegative_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countPositive was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countPositive(ByVal countPositive_Conflict As Long) As T
				Me.countPositive_Conflict = countPositive_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countMinValue was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countMinValue(ByVal countMinValue_Conflict As Long) As T
				Me.countMinValue_Conflict = countMinValue_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countMaxValue was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countMaxValue(ByVal countMaxValue_Conflict As Long) As T
				Me.countMaxValue_Conflict = countMaxValue_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countTotal was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countTotal(ByVal countTotal_Conflict As Long) As T
				Me.countTotal_Conflict = countTotal_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter histogramBuckets was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function histogramBuckets(ByVal histogramBuckets_Conflict() As Double) As T
				Me.histogramBuckets_Conflict = histogramBuckets_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter histogramBucketCounts was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function histogramBucketCounts(ByVal histogramBucketCounts_Conflict() As Long) As T
				Me.histogramBucketCounts_Conflict = histogramBucketCounts_Conflict
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter digest was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function digest(ByVal digest_Conflict As TDigest) As T
				Me.digest_Conflict = digest_Conflict
				Return CType(Me, T)
			End Function

		End Class

	End Class

End Namespace