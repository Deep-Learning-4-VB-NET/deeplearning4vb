Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

	Public Interface DataNormalization
		Inherits Normalizer(Of DataSet), DataSetPreProcessor

		''' <summary>
		''' Iterates over a dataset
		''' accumulating statistics for normalization </summary>
		''' <param name="iterator"> the iterator to use for
		'''                 collecting statistics. </param>
		Sub fit(ByVal iterator As DataSetIterator)

		Overrides Sub preProcess(ByVal toPreProcess As DataSet)

		''' <summary>
		''' Transform the dataset </summary>
		''' <param name="features"> the features to pre process </param>
		Sub transform(ByVal features As INDArray)

		''' <summary>
		''' Transform the features, with an optional mask array </summary>
		''' <param name="features"> the features to pre process </param>
		''' <param name="featuresMask"> the mask array to pre process </param>
		Sub transform(ByVal features As INDArray, ByVal featuresMask As INDArray)

		''' <summary>
		''' Transform the labels. If <seealso cref="isFitLabel()"/> == false, this is a no-op
		''' </summary>
		Sub transformLabel(ByVal labels As INDArray)

		''' <summary>
		''' Transform the labels. If <seealso cref="isFitLabel()"/> == false, this is a no-op
		''' </summary>
		Sub transformLabel(ByVal labels As INDArray, ByVal labelsMask As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified features array
		''' </summary>
		''' <param name="features">    Features to revert the normalization on </param>
		Sub revertFeatures(ByVal features As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified features array
		''' </summary>
		''' <param name="features">    Features to revert the normalization on </param>
		''' <param name="featuresMask"> </param>
		Sub revertFeatures(ByVal features As INDArray, ByVal featuresMask As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabels()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels">    Labels array to revert the normalization on </param>
		Sub revertLabels(ByVal labels As INDArray)

		''' <summary>
		''' Undo (revert) the normalization applied by this DataNormalization instance to the specified labels array.
		''' If labels normalization is disabled (i.e., <seealso cref="isFitLabels()"/> == false) then this is a no-op.
		''' Can also be used to undo normalization for network output arrays, in the case of regression.
		''' </summary>
		''' <param name="labels">    Labels array to revert the normalization on </param>
		''' <param name="labelsMask"> Labels mask array (may be null) </param>
		Sub revertLabels(ByVal labels As INDArray, ByVal labelsMask As INDArray)

		''' <summary>
		''' Flag to specify if the labels/outputs in the dataset should be also normalized. Default value is usually false.
		''' </summary>
		Sub fitLabel(ByVal fitLabels As Boolean)

		''' <summary>
		''' Whether normalization for the labels is also enabled. Most commonly used for regression, not classification.
		''' </summary>
		''' <returns> True if labels will be </returns>
		ReadOnly Property FitLabel As Boolean
	End Interface

End Namespace