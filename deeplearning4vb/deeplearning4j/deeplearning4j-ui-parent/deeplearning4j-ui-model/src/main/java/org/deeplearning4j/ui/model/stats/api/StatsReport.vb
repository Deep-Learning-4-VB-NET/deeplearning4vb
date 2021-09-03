Imports System
Imports System.Collections.Generic
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports org.nd4j.common.primitives

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


	Public Interface StatsReport
		Inherits Persistable

		Sub reportIDs(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long)

		''' <summary>
		''' Report the current iteration number
		''' </summary>
		Sub reportIterationCount(ByVal iterationCount As Integer)

		''' <summary>
		''' Get the current iteration number
		''' </summary>
		ReadOnly Property IterationCount As Integer

		''' <summary>
		''' Report the number of milliseconds required to calculate all of the stats. This is effectively the
		''' amount of listener overhead
		''' </summary>
		Sub reportStatsCollectionDurationMS(ByVal statsCollectionDurationMS As Integer)

		''' <summary>
		''' Get the number of millisecons required to calculate al of the stats. This is effectively the amount of
		''' listener overhead.
		''' </summary>
		ReadOnly Property StatsCollectionDurationMs As Integer

		''' <summary>
		''' Report model score at the current iteration
		''' </summary>
		Sub reportScore(ByVal currentScore As Double)

		''' <summary>
		''' Get the score at the current iteration
		''' </summary>
		ReadOnly Property Score As Double

		''' <summary>
		''' Report the learning rates by parameter
		''' </summary>
		Sub reportLearningRates(ByVal learningRatesByParam As IDictionary(Of String, Double))

		''' <summary>
		''' Get the learning rates by parameter
		''' </summary>
		ReadOnly Property LearningRates As IDictionary(Of String, Double)


		'--- Performance and System Stats ---

		''' <summary>
		''' Report the memory stats at this iteration
		''' </summary>
		''' <param name="jvmCurrentBytes">     Current bytes used by the JVM </param>
		''' <param name="jvmMaxBytes">         Max bytes usable by the JVM (heap) </param>
		''' <param name="offHeapCurrentBytes"> Current off-heap bytes used </param>
		''' <param name="offHeapMaxBytes">     Maximum off-heap bytes </param>
		''' <param name="deviceCurrentBytes">  Current bytes used by each device (GPU, etc). May be null if no devices are present </param>
		''' <param name="deviceMaxBytes">      Maximum bytes for each device (GPU, etc). May be null if no devices are present </param>
		Sub reportMemoryUse(ByVal jvmCurrentBytes As Long, ByVal jvmMaxBytes As Long, ByVal offHeapCurrentBytes As Long, ByVal offHeapMaxBytes As Long, ByVal deviceCurrentBytes() As Long, ByVal deviceMaxBytes() As Long)

		''' <summary>
		''' Get JVM memory - current bytes used
		''' </summary>
		ReadOnly Property JvmCurrentBytes As Long

		''' <summary>
		''' Get JVM memory - max available bytes
		''' </summary>
		ReadOnly Property JvmMaxBytes As Long

		''' <summary>
		''' Get off-heap memory - current bytes used
		''' </summary>
		ReadOnly Property OffHeapCurrentBytes As Long

		''' <summary>
		''' Get off-heap memory - max available bytes
		''' </summary>
		ReadOnly Property OffHeapMaxBytes As Long

		''' <summary>
		''' Get device (GPU, etc) current bytes - may be null if no compute devices are present in the system
		''' </summary>
		ReadOnly Property DeviceCurrentBytes As Long()

		''' <summary>
		''' Get device (GPU, etc) maximum bytes - may be null if no compute devices are present in the system
		''' </summary>
		ReadOnly Property DeviceMaxBytes As Long()

		''' <summary>
		''' Report the performance stats (since the last report)
		''' </summary>
		''' <param name="totalRuntimeMs">       Overall runtime since initialization </param>
		''' <param name="totalExamples">        Total examples processed since initialization </param>
		''' <param name="totalMinibatches">     Total number of minibatches (iterations) since initialization </param>
		''' <param name="examplesPerSecond">    Examples per second since last report </param>
		''' <param name="minibatchesPerSecond"> Minibatches per second since last report </param>
		Sub reportPerformance(ByVal totalRuntimeMs As Long, ByVal totalExamples As Long, ByVal totalMinibatches As Long, ByVal examplesPerSecond As Double, ByVal minibatchesPerSecond As Double)

		''' <summary>
		''' Get the total runtime since listener/model initialization
		''' </summary>
		ReadOnly Property TotalRuntimeMs As Long

		''' <summary>
		''' Get total number of examples that have been processed since initialization
		''' </summary>
		ReadOnly Property TotalExamples As Long

		''' <summary>
		''' Get the total number of minibatches that have been processed since initialization
		''' </summary>
		ReadOnly Property TotalMinibatches As Long

		''' <summary>
		''' Get examples per second since the last report
		''' </summary>
		ReadOnly Property ExamplesPerSecond As Double

		''' <summary>
		''' Get the number of minibatches per second, since the last report
		''' </summary>
		ReadOnly Property MinibatchesPerSecond As Double

		''' <summary>
		''' Report Garbage collection stats
		''' </summary>
		''' <param name="gcName">       Garbage collector name </param>
		''' <param name="deltaGCCount"> Change in the total number of garbage collections, since last report </param>
		''' <param name="deltaGCTime">  Change in the amount of time (milliseconds) for garbage collection, since last report </param>
		Sub reportGarbageCollection(ByVal gcName As String, ByVal deltaGCCount As Integer, ByVal deltaGCTime As Integer)

		''' <summary>
		''' Get the garbage collection stats: Pair contains GC name and the delta count/time values
		''' </summary>
		ReadOnly Property GarbageCollectionStats As IList(Of Pair(Of String, Integer()))

		'--- Histograms ---

		''' <summary>
		''' Report histograms for all parameters, for a given <seealso cref="StatsType"/>
		''' </summary>
		''' <param name="statsType"> StatsType: Parameters, Updates, Activations </param>
		''' <param name="histogram"> Histogram values for all parameters </param>
		Sub reportHistograms(ByVal statsType As StatsType, ByVal histogram As IDictionary(Of String, Histogram))

		''' <summary>
		''' Get the histograms for all parameters, for a given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type (Params/updatse/activations) to get histograms for </param>
		''' <returns> Histograms by parameter name, or null if not available </returns>
		Function getHistograms(ByVal statsType As StatsType) As IDictionary(Of String, Histogram)

		'--- Summary Stats: Mean, Variance, Mean Magnitudes ---

		''' <summary>
		''' Report the mean values for each parameter, the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type to report </param>
		''' <param name="mean">      Map of mean values, by parameter </param>
		Sub reportMean(ByVal statsType As StatsType, ByVal mean As IDictionary(Of String, Double))

		''' <summary>
		''' Get the mean values for each parameter for the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type to get mean values for </param>
		''' <returns> Map of mean values by parameter </returns>
		Function getMean(ByVal statsType As StatsType) As IDictionary(Of String, Double)

		''' <summary>
		''' Report the standard deviation values for each parameter for the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type to report std. dev values for </param>
		''' <param name="stdev">     Map of std dev values by parameter </param>
		Sub reportStdev(ByVal statsType As StatsType, ByVal stdev As IDictionary(Of String, Double))

		''' <summary>
		''' Get the standard deviation values for each parameter for the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type to get std dev values for </param>
		''' <returns> Map of stdev values by parameter </returns>
		Function getStdev(ByVal statsType As StatsType) As IDictionary(Of String, Double)

		''' <summary>
		''' Report the mean magnitude values for each parameter for the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType">      Stats type to report mean magnitude values for </param>
		''' <param name="meanMagnitudes"> Map of mean magnitude values by parameter </param>
		Sub reportMeanMagnitudes(ByVal statsType As StatsType, ByVal meanMagnitudes As IDictionary(Of String, Double))

		''' <summary>
		''' Report any metadata for the DataSet
		''' </summary>
		''' <param name="dataSetMetaData"> MetaData for the DataSet </param>
		''' <param name="metaDataClass">   Class of the metadata. Can be later retieved using <seealso cref="getDataSetMetaDataClassName()"/> </param>
		Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As Type)

		''' <summary>
		''' Report any metadata for the DataSet
		''' </summary>
		''' <param name="dataSetMetaData"> MetaData for the DataSet </param>
		''' <param name="metaDataClass">   Class of the metadata. Can be later retieved using <seealso cref="getDataSetMetaDataClassName()"/> </param>
		Sub reportDataSetMetaData(ByVal dataSetMetaData As IList(Of Serializable), ByVal metaDataClass As String)

		''' <summary>
		''' Get the mean magnitude values for each parameter for the given StatsType (Parameters/Updates/Activations)
		''' </summary>
		''' <param name="statsType"> Stats type to get mean magnitude values for </param>
		''' <returns> Map of mean magnitude values by parameter </returns>
		Function getMeanMagnitudes(ByVal statsType As StatsType) As IDictionary(Of String, Double)

		''' <summary>
		''' Get the DataSet metadata, if any (null otherwise).
		''' Note: due to serialization issues, this may in principle throw an unchecked exception related
		''' to class availability, serialization etc.
		''' </summary>
		''' <returns> List of DataSet metadata, if any. </returns>
		ReadOnly Property DataSetMetaData As IList(Of Serializable)

		''' <summary>
		''' Get the class
		''' 
		''' @return
		''' </summary>
		ReadOnly Property DataSetMetaDataClassName As String

		''' <summary>
		''' Return whether the score is present (has been reported)
		''' </summary>
		Function hasScore() As Boolean

		''' <summary>
		''' Return whether the learning rates are present (have been reported)
		''' </summary>
		Function hasLearningRates() As Boolean

		''' <summary>
		''' Return whether memory use has been reported
		''' </summary>
		Function hasMemoryUse() As Boolean

		''' <summary>
		''' Return whether performance stats (total time, total examples etc) have been reported
		''' </summary>
		Function hasPerformance() As Boolean

		''' <summary>
		''' Return whether garbage collection information has been reported
		''' </summary>
		Function hasGarbageCollection() As Boolean

		''' <summary>
		''' Return whether histograms have been reported, for the given stats type (Parameters, Updates, Activations)
		''' </summary>
		''' <param name="statsType"> Stats type </param>
		Function hasHistograms(ByVal statsType As StatsType) As Boolean

		''' <summary>
		''' Return whether the summary stats (mean, standard deviation, mean magnitudes) have been reported for the
		''' given stats type (Parameters, Updates, Activations)
		''' </summary>
		''' <param name="statsType">   stats type (Parameters, Updates, Activations) </param>
		''' <param name="summaryType"> Summary statistic type (mean, stdev, mean magnitude) </param>
		Function hasSummaryStats(ByVal statsType As StatsType, ByVal summaryType As SummaryType) As Boolean


		''' <summary>
		''' Return whether any DataSet metadata is present or not
		''' </summary>
		''' <returns> True if DataSet metadata is present </returns>
		Function hasDataSetMetaData() As Boolean
	End Interface

End Namespace