'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.common.tests.tags

	Public Class TagNames

		Public Const SAMEDIFF As String = "samediff" 'tests related to samediff
		Public Const RNG As String = "rng" 'tests related to RNG
		Public Const JAVA_ONLY As String = "java-only" 'tests with only pure java involved
		Public Const FILE_IO As String = "file-io" ' tests with file i/o
		Public Const DL4J_OLD_API As String = "dl4j-old-api" 'tests involving old dl4j api
		Public Const WORKSPACES As String = "workspaces" 'tests involving workspaces
		Public Const MULTI_THREADED As String = "multi-threaded" 'tests involving multi threading
		Public Const TRAINING As String = "training" 'tests related to training models
		Public Const LOSS_FUNCTIONS As String = "loss-functions" 'tests related to loss functions
		Public Const UI As String = "ui" 'ui related tests
		Public Const EVAL_METRICS As String = "model-eval-metrics" 'model evaluation metrics related
		Public Const CUSTOM_FUNCTIONALITY As String = "custom-functionality" 'tests related to custom ops, loss functions, layers
		Public Const JACKSON_SERDE As String = "jackson-serde" 'tests related to jackson serialization
		Public Const NDARRAY_INDEXING As String = "ndarray-indexing" 'tests related to ndarray slicing
		Public Const NDARRAY_SERDE As String = "ndarray-serde" 'tests related to ndarray serialization
		Public Const COMPRESSION As String = "compression" 'tests related to compression
		Public Const NDARRAY_ETL As String = "ndarray-etl" 'tests related to data preparation such as transforms and normalization
		Public Const MANUAL As String = "manual" 'tests related to running manually
		Public Const SPARK As String = "spark" 'tests related to apache spark
		Public Const DIST_SYSTEMS As String = "distributed-systems"
		Public Const SOLR As String = "solr"
		Public Const KERAS As String = "keras"
		Public Const PYTHON As String = "python"
		Public Const LONG_TEST As String = "long-running-test"
		Public Const NEEDS_VERIFY As String = "needs-verify" 'tests that need verification of issue
		Public Const LARGE_RESOURCES As String = "large-resources"
	End Class

End Namespace