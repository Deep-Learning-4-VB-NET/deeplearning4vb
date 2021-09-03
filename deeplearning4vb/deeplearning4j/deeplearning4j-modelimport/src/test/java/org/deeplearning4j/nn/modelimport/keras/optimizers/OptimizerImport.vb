Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports KerasModelBuilder = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

Namespace org.deeplearning4j.nn.modelimport.keras.optimizers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class OptimizerImport extends org.deeplearning4j.BaseDL4JTest
	Public Class OptimizerImport
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importAdam() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importAdam()
			importSequential("modelimport/keras/optimizers/adam.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importAdaMax() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importAdaMax()
			importSequential("modelimport/keras/optimizers/adamax.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importAdaGrad() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importAdaGrad()
			importSequential("modelimport/keras/optimizers/adagrad.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importAdaDelta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importAdaDelta()
			importSequential("modelimport/keras/optimizers/adadelta.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importSGD() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importSGD()
			importSequential("modelimport/keras/optimizers/sgd.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importRmsProp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importRmsProp()
			importSequential("modelimport/keras/optimizers/rmsprop.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void importNadam() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importNadam()
			importSequential("modelimport/keras/optimizers/nadam.h5")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void importSequential(String modelPath) throws Exception
		Private Sub importSequential(ByVal modelPath As String)
			Dim modelFile As File = DL4JFileUtils.createTempFile("tempModel", ".h5")
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Dim builder As KerasModelBuilder = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(False)

				Dim model As KerasSequentialModel = builder.buildSequential()
				model.MultiLayerNetwork
			End Using
		End Sub
	End Class

End Namespace