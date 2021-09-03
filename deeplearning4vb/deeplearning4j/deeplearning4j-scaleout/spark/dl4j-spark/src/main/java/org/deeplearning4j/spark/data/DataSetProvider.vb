Imports SparkContext = org.apache.spark.SparkContext
Imports RDD = org.apache.spark.rdd.RDD
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.data

	''' <summary>
	''' A provider for an <seealso cref="DataSet"/>
	''' rdd.
	''' @author Adam Gibson
	''' </summary>
	Public Interface DataSetProvider

		''' <summary>
		''' Return an rdd of type dataset
		''' @return
		''' </summary>
		Function data(ByVal sparkContext As SparkContext) As RDD(Of DataSet)

		''' <summary>
		''' (Optional) The transform process
		''' for the dataset provider.
		''' @return
		''' </summary>
		Function transformProcess() As TransformProcess

	End Interface

End Namespace