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

Namespace org.deeplearning4j.ui.model.stats.api


	Public Interface StatsUpdateConfiguration

		''' <summary>
		''' Get the reporting frequency, in terms of listener calls
		''' </summary>
		Function reportingFrequency() As Integer

		'TODO
		'boolean useNTPTimeSource();

		'--- Performance and System Stats ---

		''' <summary>
		''' Should performance stats be collected/reported?
		''' Total time, total examples, total batches, Minibatches/second, examples/second
		''' </summary>
		Function collectPerformanceStats() As Boolean

		''' <summary>
		''' Should JVM, off-heap and memory stats be collected/reported?
		''' </summary>
		Function collectMemoryStats() As Boolean

		''' <summary>
		''' Should garbage collection stats be collected and reported?
		''' </summary>
		Function collectGarbageCollectionStats() As Boolean

		'TODO
		'    boolean collectDataSetMetaData();

		'--- General ---

		''' <summary>
		''' Should per-parameter type learning rates be collected and reported?
		''' </summary>
		Function collectLearningRates() As Boolean

		'--- Histograms ---

		''' <summary>
		''' Should histograms (per parameter type, or per layer for activations) of the given type be collected?
		''' </summary>
		''' <param name="type"> Stats type: Parameters, Updates, Activations </param>
		Function collectHistograms(ByVal type As StatsType) As Boolean

		''' <summary>
		''' Get the number of histogram bins to use for the given type (for use with <seealso cref="collectHistograms(StatsType)"/>
		''' </summary>
		''' <param name="type"> Stats type: Parameters, Updates, Activatinos </param>
		Function numHistogramBins(ByVal type As StatsType) As Integer

		'--- Summary Stats: Mean, Variance, Mean Magnitudes ---

		''' <summary>
		''' Should the mean values (per parameter type, or per layer for activations) be collected?
		''' </summary>
		''' <param name="type"> Stats type: Parameters, Updates, Activations </param>
		Function collectMean(ByVal type As StatsType) As Boolean

		''' <summary>
		''' Should the standard devication values (per parameter type, or per layer for activations) be collected?
		''' </summary>
		''' <param name="type"> Stats type: Parameters, Updates, Activations </param>
		Function collectStdev(ByVal type As StatsType) As Boolean

		''' <summary>
		''' Should the mean magnitude values (per parameter type, or per layer for activations) be collected?
		''' </summary>
		''' <param name="type"> Stats type: Parameters, Updates, Activations </param>
		Function collectMeanMagnitudes(ByVal type As StatsType) As Boolean

	End Interface

End Namespace