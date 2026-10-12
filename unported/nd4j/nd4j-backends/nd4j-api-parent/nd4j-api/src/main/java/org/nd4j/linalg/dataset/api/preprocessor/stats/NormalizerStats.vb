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

Namespace org.nd4j.linalg.dataset.api.preprocessor.stats


	Public Interface NormalizerStats
		Friend Interface Builder(Of S As NormalizerStats)
			Function addFeatures(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As Builder(Of S)

			''' <summary>
			''' Add the labels of a DataSet to the statistics
			''' </summary>
			Function addLabels(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As Builder(Of S)

			''' <summary>
			''' Add rows of data to the statistics
			''' </summary>
			''' <param name="data"> the matrix containing multiple rows of data to include </param>
			''' <param name="mask"> (optionally) the mask of the data, useful for e.g. time series </param>
			Function add(ByVal data As INDArray, ByVal mask As INDArray) As Builder(Of S)

			''' <summary>
			''' DynamicCustomOpsBuilder pattern
			''' @return
			''' </summary>
			Function build() As S
		End Interface
	End Interface

End Namespace