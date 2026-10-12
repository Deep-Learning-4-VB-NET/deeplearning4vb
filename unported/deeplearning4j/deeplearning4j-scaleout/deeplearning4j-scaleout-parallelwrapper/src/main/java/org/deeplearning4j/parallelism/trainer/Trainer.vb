Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Model = org.deeplearning4j.nn.api.Model
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.deeplearning4j.parallelism.trainer

	Public Interface Trainer
		Inherits ThreadStart

		''' <summary>
		''' Train on a <seealso cref="MultiDataSet"/> </summary>
		''' <param name="dataSet"> the data set to train on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void feedMultiDataSet(@NonNull MultiDataSet dataSet, long etlTime);
		Sub feedMultiDataSet(ByVal dataSet As MultiDataSet, ByVal etlTime As Long)


		''' <summary>
		''' Train on a <seealso cref="DataSet"/> </summary>
		''' <param name="dataSet"> the data set to train on </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void feedDataSet(@NonNull DataSet dataSet, long etlTime);
		Sub feedDataSet(ByVal dataSet As DataSet, ByVal etlTime As Long)

		''' <summary>
		''' This method updates replicated model params </summary>
		''' <param name="params"> </param>
		Sub updateModelParams(ByVal params As INDArray)

		''' <summary>
		''' This method updates updater params of the replicated model </summary>
		''' <param name="params"> </param>
		Sub updateUpdaterParams(ByVal params As INDArray)

		''' <summary>
		''' THe current model for the trainer </summary>
		''' <returns> the current  <seealso cref="Model"/>
		''' for the worker </returns>
		ReadOnly Property Model As Model

		''' <summary>
		''' Update the current <seealso cref="Model"/>
		''' for the worker </summary>
		''' <param name="model"> the new model for this worker </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: void updateModel(@NonNull Model model);
		Sub updateModel(ByVal model As Model)

		ReadOnly Property Running As Boolean

		ReadOnly Property Uuid As String

		''' <summary>
		''' Shutdown this worker
		''' </summary>
		Sub shutdown()

		''' <summary>
		''' Block the main thread
		''' till the trainer is up and running.
		''' </summary>
		Sub waitTillRunning()

		''' <summary>
		''' Set the <seealso cref="System.Threading.Thread.UncaughtExceptionHandler"/>
		''' for this <seealso cref="Trainer"/> </summary>
		''' <param name="handler"> the handler for uncaught errors </param>
		WriteOnly Property UncaughtExceptionHandler As Thread.UncaughtExceptionHandler

		''' <summary>
		''' Start this trainer
		''' </summary>
		Sub start()

		''' <summary>
		''' This method returns TRUE if this Trainer implementation assumes periodic aver
		''' @return
		''' </summary>
		Function averagingRequired() As Boolean
	End Interface

End Namespace