Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

	Public Interface MultiDataNormalization
		Inherits Normalizer(Of MultiDataSet), MultiDataSetPreProcessor

		''' <summary>
		''' Iterates over a dataset
		''' accumulating statistics for normalization
		''' </summary>
		''' <param name="iterator"> the iterator to use for
		'''                 collecting statistics. </param>
		Sub fit(ByVal iterator As MultiDataSetIterator)

		Overrides Sub preProcess(ByVal multiDataSet As MultiDataSet)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified features array
		''' </summary>
		''' <param name="features">    Features to revert the normalization on </param>
		''' <param name="featuresMask"> </param>
		Sub revertFeatures(ByVal features() As INDArray, ByVal featuresMask() As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified features array
		''' </summary>
		''' <param name="features"> Features to revert the normalization on </param>
		Sub revertFeatures(ByVal features() As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels">    Labels array to revert the normalization on </param>
		''' <param name="labelsMask"> Labels mask array (may be null) </param>
		Sub revertLabels(ByVal labels() As INDArray, ByVal labelsMask() As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabel()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels"> Labels array to revert the normalization on </param>
		Sub revertLabels(ByVal labels() As INDArray)
	End Interface

End Namespace