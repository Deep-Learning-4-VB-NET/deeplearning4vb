Imports System
Imports System.Collections.Generic
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.nd4j.evaluation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) public interface IEvaluation<T extends IEvaluation> extends java.io.Serializable
	Public Interface IEvaluation(Of T As IEvaluation)


		''' 
		''' <param name="labels"> </param>
		''' <param name="networkPredictions"> </param>
		Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray)

		''' 
		''' <param name="labels"> </param>
		''' <param name="networkPredictions"> </param>
		''' <param name="recordMetaData"> </param>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void eval(org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray networkPredictions, java.util.List<? extends java.io.Serializable> recordMetaData);
		Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal recordMetaData As IList(Of T1))

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void eval(org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray networkPredictions, org.nd4j.linalg.api.ndarray.INDArray maskArray, java.util.List<? extends java.io.Serializable> recordMetaData);
		Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))

		''' 
		''' <param name="labels"> </param>
		''' <param name="networkPredictions"> </param>
		''' <param name="maskArray"> </param>
		Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray)


		''' @deprecated Use <seealso cref="eval(INDArray, INDArray)"/> 
		<Obsolete("Use <seealso cref=""eval(INDArray, INDArray)""/>")>
		Sub evalTimeSeries(ByVal labels As INDArray, ByVal predicted As INDArray)

		''' @deprecated Use <seealso cref="eval(INDArray, INDArray, INDArray)"/> 
		<Obsolete("Use <seealso cref=""eval(INDArray, INDArray, INDArray)""/>")>
		Sub evalTimeSeries(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal labelsMaskArray As INDArray)

		''' 
		''' <param name="other"> </param>
		Sub merge(ByVal other As T)

		''' 
		Sub reset()

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function stats() As String

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function toJson() As String

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Function toYaml() As String

		''' <summary>
		''' Get the value of a given metric for this evaluation.
		''' </summary>
		Function getValue(ByVal metric As IMetric) As Double

		''' <summary>
		''' Get a new instance of this evaluation, with the same configuration but no data.
		''' </summary>
		Function newInstance() As T
	End Interface

End Namespace