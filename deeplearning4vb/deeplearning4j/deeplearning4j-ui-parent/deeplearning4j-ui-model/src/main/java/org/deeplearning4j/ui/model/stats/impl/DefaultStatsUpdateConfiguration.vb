Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports StatsType = org.deeplearning4j.ui.model.stats.api.StatsType
Imports StatsUpdateConfiguration = org.deeplearning4j.ui.model.stats.api.StatsUpdateConfiguration

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

Namespace org.deeplearning4j.ui.model.stats.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DefaultStatsUpdateConfiguration implements org.deeplearning4j.ui.model.stats.api.StatsUpdateConfiguration
	<Serializable>
	Public Class DefaultStatsUpdateConfiguration
		Implements StatsUpdateConfiguration

		Public Const DEFAULT_REPORTING_FREQUENCY As Integer = 10

'JAVA TO VB CONVERTER NOTE: The field reportingFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private reportingFrequency_Conflict As Integer = DEFAULT_REPORTING_FREQUENCY
'JAVA TO VB CONVERTER NOTE: The field collectPerformanceStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private collectPerformanceStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMemoryStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private collectMemoryStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectGarbageCollectionStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private collectGarbageCollectionStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectLearningRates was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private collectLearningRates_Conflict As Boolean = True
		Private collectHistogramsParameters As Boolean = True
		Private collectHistogramsGradients As Boolean = True
		Private collectHistogramsUpdates As Boolean = True
		Private collectHistogramsActivations As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field numHistogramBins was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numHistogramBins_Conflict As Integer = 20
		Private collectMeanParameters As Boolean = True
		Private collectMeanGradients As Boolean = True
		Private collectMeanUpdates As Boolean = True
		Private collectMeanActivations As Boolean = True
		Private collectStdevParameters As Boolean = True
		Private collectStdevGradients As Boolean = True
		Private collectStdevUpdates As Boolean = True
		Private collectStdevActivations As Boolean = True
		Private collectMeanMagnitudesParameters As Boolean = True
		Private collectMeanMagnitudesGradients As Boolean = True
		Private collectMeanMagnitudesUpdates As Boolean = True
		Private collectMeanMagnitudesActivations As Boolean = True

		Private Sub New(ByVal b As Builder)
			Me.reportingFrequency_Conflict = b.reportingFrequency_Conflict
			Me.collectPerformanceStats_Conflict = b.collectPerformanceStats_Conflict
			Me.collectMemoryStats_Conflict = b.collectMemoryStats_Conflict
			Me.collectGarbageCollectionStats_Conflict = b.collectGarbageCollectionStats_Conflict
			Me.collectLearningRates_Conflict = b.collectLearningRates_Conflict
			Me.collectHistogramsParameters = b.collectHistogramsParameters_Conflict
			Me.collectHistogramsGradients = b.collectHistogramsGradients_Conflict
			Me.collectHistogramsUpdates = b.collectHistogramsUpdates_Conflict
			Me.collectHistogramsActivations = b.collectHistogramsActivations_Conflict
			Me.numHistogramBins_Conflict = b.numHistogramBins_Conflict
			Me.collectMeanParameters = b.collectMeanParameters_Conflict
			Me.collectMeanGradients = b.collectMeanGradients_Conflict
			Me.collectMeanUpdates = b.collectMeanUpdates_Conflict
			Me.collectMeanActivations = b.collectMeanActivations_Conflict
			Me.collectStdevParameters = b.collectStdevParameters_Conflict
			Me.collectStdevGradients = b.collectStdevGradients_Conflict
			Me.collectStdevUpdates = b.collectStdevUpdates_Conflict
			Me.collectStdevActivations = b.collectStdevActivations_Conflict
			Me.collectMeanMagnitudesParameters = b.collectMeanMagnitudesParameters_Conflict
			Me.collectMeanMagnitudesGradients = b.collectMeanMagnitudesGradients_Conflict
			Me.collectMeanMagnitudesUpdates = b.collectMeanMagnitudesUpdates_Conflict
			Me.collectMeanMagnitudesActivations = b.collectMeanMagnitudesActivations_Conflict
		End Sub

		Public Overridable Function reportingFrequency() As Integer Implements StatsUpdateConfiguration.reportingFrequency
			Return reportingFrequency_Conflict
		End Function

		Public Overridable Function collectPerformanceStats() As Boolean Implements StatsUpdateConfiguration.collectPerformanceStats
			Return collectPerformanceStats_Conflict
		End Function

		Public Overridable Function collectMemoryStats() As Boolean Implements StatsUpdateConfiguration.collectMemoryStats
			Return collectMemoryStats_Conflict
		End Function

		Public Overridable Function collectGarbageCollectionStats() As Boolean Implements StatsUpdateConfiguration.collectGarbageCollectionStats
			Return collectGarbageCollectionStats_Conflict
		End Function

		Public Overridable Function collectLearningRates() As Boolean Implements StatsUpdateConfiguration.collectLearningRates
			Return collectLearningRates_Conflict
		End Function

		Public Overridable Function collectHistograms(ByVal type As StatsType) As Boolean
			Select Case type
				Case StatsType.Parameters
					Return collectHistogramsParameters
				Case StatsType.Gradients
					Return collectStdevGradients
				Case StatsType.Updates
					Return collectHistogramsUpdates
				Case StatsType.Activations
					Return collectHistogramsActivations
			End Select
			Return False
		End Function

		Public Overridable Function numHistogramBins(ByVal type As StatsType) As Integer
			Return numHistogramBins_Conflict
		End Function

		Public Overridable Function collectMean(ByVal type As StatsType) As Boolean
			Select Case type
				Case StatsType.Parameters
					Return collectMeanParameters
				Case StatsType.Gradients
					Return collectMeanGradients
				Case StatsType.Updates
					Return collectMeanUpdates
				Case StatsType.Activations
					Return collectMeanActivations
			End Select
			Return False
		End Function

		Public Overridable Function collectStdev(ByVal type As StatsType) As Boolean
			Select Case type
				Case StatsType.Parameters
					Return collectStdevParameters
				Case StatsType.Gradients
					Return collectStdevGradients
				Case StatsType.Updates
					Return collectStdevUpdates
				Case StatsType.Activations
					Return collectStdevActivations
			End Select
			Return False
		End Function

		Public Overridable Function collectMeanMagnitudes(ByVal type As StatsType) As Boolean
			Select Case type
				Case StatsType.Parameters
					Return collectMeanMagnitudesParameters
				Case StatsType.Gradients
					Return collectMeanMagnitudesGradients
				Case StatsType.Updates
					Return collectMeanMagnitudesUpdates
				Case StatsType.Activations
					Return collectMeanMagnitudesActivations
			End Select
			Return False
		End Function

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field reportingFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportingFrequency_Conflict As Integer = DEFAULT_REPORTING_FREQUENCY
'JAVA TO VB CONVERTER NOTE: The field collectPerformanceStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectPerformanceStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMemoryStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMemoryStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectGarbageCollectionStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectGarbageCollectionStats_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectLearningRates was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectLearningRates_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectHistogramsParameters was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectHistogramsParameters_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectHistogramsGradients was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectHistogramsGradients_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectHistogramsUpdates was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectHistogramsUpdates_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectHistogramsActivations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectHistogramsActivations_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field numHistogramBins was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend numHistogramBins_Conflict As Integer = 20
'JAVA TO VB CONVERTER NOTE: The field collectMeanParameters was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanParameters_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanGradients was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanGradients_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanUpdates was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanUpdates_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanActivations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanActivations_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectStdevParameters was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectStdevParameters_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectStdevGradients was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectStdevGradients_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectStdevUpdates was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectStdevUpdates_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectStdevActivations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectStdevActivations_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanMagnitudesParameters was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanMagnitudesParameters_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanMagnitudesGradients was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanMagnitudesGradients_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanMagnitudesUpdates was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanMagnitudesUpdates_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field collectMeanMagnitudesActivations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend collectMeanMagnitudesActivations_Conflict As Boolean = True

'JAVA TO VB CONVERTER NOTE: The parameter reportingFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportingFrequency(ByVal reportingFrequency_Conflict As Integer) As Builder
				Me.reportingFrequency_Conflict = reportingFrequency_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectPerformanceStats was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectPerformanceStats(ByVal collectPerformanceStats_Conflict As Boolean) As Builder
				Me.collectPerformanceStats_Conflict = collectPerformanceStats_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMemoryStats was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMemoryStats(ByVal collectMemoryStats_Conflict As Boolean) As Builder
				Me.collectMemoryStats_Conflict = collectMemoryStats_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectGarbageCollectionStats was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectGarbageCollectionStats(ByVal collectGarbageCollectionStats_Conflict As Boolean) As Builder
				Me.collectGarbageCollectionStats_Conflict = collectGarbageCollectionStats_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectLearningRates was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectLearningRates(ByVal collectLearningRates_Conflict As Boolean) As Builder
				Me.collectLearningRates_Conflict = collectLearningRates_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectHistogramsParameters was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectHistogramsParameters(ByVal collectHistogramsParameters_Conflict As Boolean) As Builder
				Me.collectHistogramsParameters_Conflict = collectHistogramsParameters_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectHistogramsGradients was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectHistogramsGradients(ByVal collectHistogramsGradients_Conflict As Boolean) As Builder
				Me.collectHistogramsGradients_Conflict = collectHistogramsGradients_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectHistogramsUpdates was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectHistogramsUpdates(ByVal collectHistogramsUpdates_Conflict As Boolean) As Builder
				Me.collectHistogramsUpdates_Conflict = collectHistogramsUpdates_Conflict
				Return Me
			End Function

			Public Overridable Function collectHistogramsActivations(ByVal isCollectHistogramsActivations As Boolean) As Builder
				Me.collectHistogramsActivations_Conflict = isCollectHistogramsActivations
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter numHistogramBins was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function numHistogramBins(ByVal numHistogramBins_Conflict As Integer) As Builder
				Me.numHistogramBins_Conflict = numHistogramBins_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanParameters was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanParameters(ByVal collectMeanParameters_Conflict As Boolean) As Builder
				Me.collectMeanParameters_Conflict = collectMeanParameters_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanGradients was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanGradients(ByVal collectMeanGradients_Conflict As Boolean) As Builder
				Me.collectMeanGradients_Conflict = collectMeanGradients_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanUpdates was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanUpdates(ByVal collectMeanUpdates_Conflict As Boolean) As Builder
				Me.collectMeanUpdates_Conflict = collectMeanUpdates_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanActivations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanActivations(ByVal collectMeanActivations_Conflict As Boolean) As Builder
				Me.collectMeanActivations_Conflict = collectMeanActivations_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectStdevParameters was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectStdevParameters(ByVal collectStdevParameters_Conflict As Boolean) As Builder
				Me.collectStdevParameters_Conflict = collectStdevParameters_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectStdevGradients was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectStdevGradients(ByVal collectStdevGradients_Conflict As Boolean) As Builder
				Me.collectStdevGradients_Conflict = collectStdevGradients_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectStdevUpdates was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectStdevUpdates(ByVal collectStdevUpdates_Conflict As Boolean) As Builder
				Me.collectStdevUpdates_Conflict = collectStdevUpdates_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectStdevActivations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectStdevActivations(ByVal collectStdevActivations_Conflict As Boolean) As Builder
				Me.collectStdevActivations_Conflict = collectStdevActivations_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanMagnitudesParameters was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanMagnitudesParameters(ByVal collectMeanMagnitudesParameters_Conflict As Boolean) As Builder
				Me.collectMeanMagnitudesParameters_Conflict = collectMeanMagnitudesParameters_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanMagnitudesGradients was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanMagnitudesGradients(ByVal collectMeanMagnitudesGradients_Conflict As Boolean) As Builder
				Me.collectMeanMagnitudesGradients_Conflict = collectMeanMagnitudesGradients_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanMagnitudesUpdates was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanMagnitudesUpdates(ByVal collectMeanMagnitudesUpdates_Conflict As Boolean) As Builder
				Me.collectMeanMagnitudesUpdates_Conflict = collectMeanMagnitudesUpdates_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter collectMeanMagnitudesActivations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectMeanMagnitudesActivations(ByVal collectMeanMagnitudesActivations_Conflict As Boolean) As Builder
				Me.collectMeanMagnitudesActivations_Conflict = collectMeanMagnitudesActivations_Conflict
				Return Me
			End Function

			Public Overridable Function build() As DefaultStatsUpdateConfiguration
				Return New DefaultStatsUpdateConfiguration(Me)
			End Function
		End Class
	End Class

End Namespace