Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.evaluation.custom


	Public Interface EvaluationLambda(Of T)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public T eval(org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray networkPredictions, org.nd4j.linalg.api.ndarray.INDArray maskArray, java.util.List<? extends java.io.Serializable> recordMetaData);
		Function eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1)) As T
	End Interface

End Namespace