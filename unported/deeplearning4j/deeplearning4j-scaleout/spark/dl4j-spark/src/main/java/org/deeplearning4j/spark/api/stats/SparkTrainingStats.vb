Imports System.Collections.Generic
Imports SparkContext = org.apache.spark.SparkContext
Imports ParameterAveragingTrainingMasterStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingMasterStats
Imports ParameterAveragingTrainingWorkerStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingWorkerStats
Imports EventStats = org.deeplearning4j.spark.stats.EventStats

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

Namespace org.deeplearning4j.spark.api.stats


	Public Interface SparkTrainingStats

		''' <summary>
		''' Default indentation for <seealso cref="statsAsString()"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		int PRINT_INDENT = 55;

		''' <summary>
		''' Default formatter used for <seealso cref="statsAsString()"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String DEFAULT_PRINT_FORMAT = "%-" + PRINT_INDENT + "s";

		''' <returns> Set of keys that can be used with <seealso cref="getValue(String)"/> </returns>
		ReadOnly Property KeySet As ISet(Of String)

		''' <summary>
		''' Get the statistic value for this key
		''' </summary>
		''' <param name="key"> Key to get the value for </param>
		''' <returns> Statistic for this key, or an exception if key is invalid </returns>
		Function getValue(ByVal key As String) As IList(Of EventStats)

		''' <summary>
		''' Return a short (display) name for the given key.
		''' </summary>
		''' <param name="key">    Key </param>
		''' <returns> Short/display name for key </returns>
		Function getShortNameForKey(ByVal key As String) As String

		''' <summary>
		''' When plotting statistics, we don't necessarily want to plot everything.
		''' For example, some statistics/measurements are made up multiple smaller components; it does not always make sense
		''' to plot both the larger stat, and the components that make it up
		''' </summary>
		''' <param name="key"> Key to check for default plotting behaviour </param>
		''' <returns> Whether the specified key should be included in plots by default or not </returns>
		Function defaultIncludeInPlots(ByVal key As String) As Boolean

		''' <summary>
		''' Combine the two training stats instances. Usually, the two objects must be of the same type
		''' </summary>
		''' <param name="other"> Other training stats to return </param>
		Sub addOtherTrainingStats(ByVal other As SparkTrainingStats)

		''' <summary>
		''' Return the nested training stats - if any.
		''' </summary>
		''' <returns> The nested stats, if present/applicable, or null otherwise </returns>
		ReadOnly Property NestedTrainingStats As SparkTrainingStats

		''' <summary>
		''' Get a String representation of the stats. This functionality is implemented as a separate method (as opposed to toString())
		''' as the resulting String can be very large.<br>
		''' 
		''' <b>NOTE</b>: The String representation typically includes only duration information. To get full statistics (including
		''' machine IDs, etc) use <seealso cref="getValue(String)"/> or export full data via <seealso cref="exportStatFiles(String, SparkContext)"/>
		''' </summary>
		''' <returns> A String representation of the training statistics </returns>
		Function statsAsString() As String


		''' <summary>
		''' Export the stats as a collection of files. Stats are comma-delimited (CSV) with 1 header line
		''' </summary>
		''' <param name="outputPath">    Base directory to write files to </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void exportStatFiles(String outputPath, org.apache.spark.SparkContext sc) throws java.io.IOException;
		Sub exportStatFiles(ByVal outputPath As String, ByVal sc As SparkContext)
	End Interface

End Namespace