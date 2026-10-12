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

Namespace org.deeplearning4j.common.config

	Public Class DL4JSystemProperties

		Private Sub New()
		End Sub

		''' <summary>
		''' Applicability: DL4J ModelSerializer, ModelGuesser, Keras model import<br>
		''' Description: Specify the local directory where temporary files will be written. If not specified, the default
		''' Java temporary directory (java.io.tmpdir system property) will generally be used.
		''' </summary>
		Public Const DL4J_TEMP_DIR_PROPERTY As String = "org.deeplearning4j.tempdir"

		''' <summary>
		''' Applicability: Numerous modules, including deeplearning4j-datasets and deeplearning4j-zoo<br>
		''' Description: Used to set the local location for downloaded remote resources such as datasets (like MNIST) and
		''' pretrained models in the model zoo. Default value is set via {@code new File(System.getProperty("user.home"), ".deeplearning4j")}.
		''' Setting this can be useful if the system drive has limited space/performance, a shared location for all users
		''' should be used instead, or if user.home isn't set for some reason.
		''' </summary>
		Public Const DL4J_RESOURCES_DIR_PROPERTY As String = "org.deeplearning4j.resources.directory"

		''' <summary>
		''' Applicability: Numerous modules, including deeplearning4j-datasets and deeplearning4j-zoo<br>
		''' Description: Used to set the base URL for hosting of resources such as datasets (like MNIST) and pretrained
		''' models in the model zoo. This is provided as a fallback in case the location of these files changes; it
		''' also allows for (in principle) a local mirror of these files.<br>
		''' NOTE: Changing this to a location without the same files and file structure as the DL4J resource hosting is likely
		''' to break external resource dowloading in DL4J!
		''' </summary>
		Public Const DL4J_RESOURCES_BASE_URL_PROPERTY As String = "org.deeplearning4j.resources.baseurl"

		''' <summary>
		''' Applicability: deeplearning4j-nn<br>
		''' Description: DL4J writes some crash dumps to disk when an OOM exception occurs - this functionality is enabled
		''' by default. This is to help users identify the cause of the OOM - i.e., where native memory is actually consumed.
		''' This system property can be used to disable memory crash reporting. </summary>
		''' <seealso cref= #CRASH_DUMP_OUTPUT_DIRECTORY_PROPERTY For configuring the output directory </seealso>
		Public Const CRASH_DUMP_ENABLED_PROPERTY As String = "org.deeplearning4j.crash.reporting.enabled"

		''' <summary>
		''' Applicability: deeplearning4j-nn<br>
		''' Description: DL4J writes some crash dumps to disk when an OOM exception occurs - this functionality is enabled
		''' by default. This system property can be use to customize the output directory for memory crash reporting. By default,
		''' the current working directory ({@code  System.getProperty("user.dir")} or {@code new File("")}) will be used </summary>
		''' <seealso cref= #CRASH_DUMP_ENABLED_PROPERTY To disable crash dump reporting </seealso>
		Public Const CRASH_DUMP_OUTPUT_DIRECTORY_PROPERTY As String = "org.deeplearning4j.crash.reporting.directory"

		''' <summary>
		''' Applicability: deeplearning4j-ui<br>
		''' Description: The DL4J training UI (StatsListener + UIServer.getInstance().attach(ss)) will subsample the number
		''' of chart points when a lot of data is present - i.e., only a maximum number of points will be shown on each chart.
		''' This is to reduce the UI bandwidth requirements and client-side rendering cost.
		''' To increase the number of points in charts, set this property to a larger value. Default: 512 values
		''' </summary>
		Public Const CHART_MAX_POINTS_PROPERTY As String = "org.deeplearning4j.ui.maxChartPoints"


		''' <summary>
		''' Applicability: deeplearning4j-vertx (deeplearning4j-ui)<br>
		''' Description: This property sets the port that the UI will be available on. Default port: 9000.
		''' Set to 0 for a random port.
		''' </summary>
		Public Const UI_SERVER_PORT_PROPERTY As String = "org.deeplearning4j.ui.port"

		''' <summary>
		''' Applicability: dl4j-spark_2.xx - NTPTimeSource class (mainly used in ParameterAveragingTrainingMaster when stats
		''' collection is enabled; not enabled by default)<br>
		''' Description: This sets the NTP (network time protocol) server to be used when collecting stats. Default: 0.pool.ntp.org
		''' </summary>
		Public Const NTP_SOURCE_SERVER_PROPERTY As String = "org.deeplearning4j.spark.time.NTPTimeSource.server"

		''' <summary>
		''' Applicability: dl4j-spark_2.xx - NTPTimeSource class (mainly used in ParameterAveragingTrainingMaster when stats
		''' collection is enabled; not enabled by default)<br>
		''' Description: This sets the NTP (network time protocol) update frequency in milliseconds. Default: 1800000 (30 minutes)
		''' </summary>
		Public Const NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY As String = "org.deeplearning4j.spark.time.NTPTimeSource.frequencyms"

		''' <summary>
		''' Applicability: dl4j-spark_2.xx - mainly used in ParameterAveragingTrainingMaster when stats collection is enabled;
		''' not enabled by default<br>
		''' Description: This sets the time source to use for spark stats. Default: {@code org.deeplearning4j.spark.time.NTPTimeSource}
		''' </summary>
		Public Const TIMESOURCE_CLASSNAME_PROPERTY As String = "org.deeplearning4j.spark.time.TimeSource"


		''' <summary>
		''' Applicability: {@code org.deeplearning4j.nn.layers.HelperUtils}
		''' Used in whether to disable the helpers or not.
		''' </summary>
		Public Const DISABLE_HELPER_PROPERTY As String = "org.eclipse.deeplearning4j.helpers.disable"
		Public Const HELPER_DISABLE_DEFAULT_VALUE As String = "true"
	End Class

End Namespace