Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports org.nd4j.linalg.dataset.api.preprocessor.serializer
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	Public Interface Normalizer(Of T)
		''' <summary>
		''' Fit a dataset (only compute based on the statistics from this dataset)
		''' </summary>
		''' <param name="dataSet"> the dataset to compute on </param>
		Sub fit(ByVal dataSet As T)

		''' <summary>
		''' Transform the dataset
		''' </summary>
		''' <param name="toPreProcess"> the dataset to re process </param>
		Sub transform(ByVal toPreProcess As T)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance (arrays are modified in-place)
		''' </summary>
		''' <param name="toRevert"> DataSet to revert the normalization on </param>
		Sub revert(ByVal toRevert As T)

		''' <summary>
		''' Get the enum opType of this normalizer
		''' </summary>
		''' <returns> the opType </returns>
		''' <seealso cref= NormalizerSerializerStrategy#getSupportedType() </seealso>
		Function [getType]() As NormalizerType
	End Interface

End Namespace