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

Namespace org.deeplearning4vb.common.config

    ''' <summary>Names of the system properties DL4VB reads (see <c>JavaSystem.getProperty</c>).</summary>
    Public NotInheritable Class DL4VBSystemProperties

        Private Sub New()
        End Sub

        ''' <summary>Directory used for temporary files instead of the default temporary directory.</summary>
        Public Const DL4VB_TEMP_DIR_PROPERTY As String = "org.deeplearning4vb.tempdir"

        ''' <summary>Base directory for downloaded datasets, zoo models and other resources (default: ~/.deeplearning4vb).</summary>
        Public Const DL4VB_RESOURCES_DIR_PROPERTY As String = "org.deeplearning4vb.resources.directory"

        ''' <summary>Base URL that resources are downloaded from.</summary>
        Public Const DL4VB_RESOURCES_BASE_URL_PROPERTY As String = "org.deeplearning4vb.resources.baseurl"

        ''' <summary>Whether crash dumps are written on out-of-memory errors.</summary>
        Public Const CRASH_DUMP_ENABLED_PROPERTY As String = "org.deeplearning4vb.crash.reporting.enabled"

        ''' <summary>Directory the crash dumps are written to.</summary>
        Public Const CRASH_DUMP_OUTPUT_DIRECTORY_PROPERTY As String = "org.deeplearning4vb.crash.reporting.directory"

        ''' <summary>Maximum number of points shown in the UI charts.</summary>
        Public Const CHART_MAX_POINTS_PROPERTY As String = "org.deeplearning4vb.ui.maxChartPoints"

        ''' <summary>Port of the UI server.</summary>
        Public Const UI_SERVER_PORT_PROPERTY As String = "org.deeplearning4vb.ui.port"

        ''' <summary>NTP server used by the Spark time source.</summary>
        Public Const NTP_SOURCE_SERVER_PROPERTY As String = "org.deeplearning4vb.spark.time.NTPTimeSource.server"

        ''' <summary>Update frequency (ms) of the Spark NTP time source.</summary>
        Public Const NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY As String = "org.deeplearning4vb.spark.time.NTPTimeSource.frequencyms"

        ''' <summary>Class name of the Spark time source.</summary>
        Public Const TIMESOURCE_CLASSNAME_PROPERTY As String = "org.deeplearning4vb.spark.time.TimeSource"

        ''' <summary>Set to "true" to disable the cuDNN/MKL-DNN helper implementations of layers.</summary>
        Public Const DISABLE_HELPER_PROPERTY As String = "org.deeplearning4vb.helpers.disable"
        Public Const HELPER_DISABLE_DEFAULT_VALUE As String = "true"
    End Class

End Namespace
