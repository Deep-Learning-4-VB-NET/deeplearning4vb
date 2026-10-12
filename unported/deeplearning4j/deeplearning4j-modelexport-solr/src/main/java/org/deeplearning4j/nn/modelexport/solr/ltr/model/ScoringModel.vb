Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports LeafReaderContext = org.apache.lucene.index.LeafReaderContext
Imports Explanation = org.apache.lucene.search.Explanation
Imports SolrResourceLoader = org.apache.solr.core.SolrResourceLoader
Imports Feature = org.apache.solr.ltr.feature.Feature
Imports AdapterModel = org.apache.solr.ltr.model.AdapterModel
Imports ModelException = org.apache.solr.ltr.model.ModelException
Imports Normalizer = org.apache.solr.ltr.norm.Normalizer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.nn.modelexport.solr.ltr.model


	Public Class ScoringModel
		Inherits AdapterModel

'JAVA TO VB CONVERTER NOTE: The field serializedModelFileName was renamed since Visual Basic does not allow fields to have the same name as other class members:
	  Private serializedModelFileName_Conflict As String
	  Protected Friend model As Model

	  Public Sub New(ByVal name As String, ByVal features As IList(Of Feature), ByVal norms As IList(Of Normalizer), ByVal featureStoreName As String, ByVal allFeatures As IList(Of Feature), ByVal params As IDictionary(Of String, Object))
		MyBase.New(name, features, norms, featureStoreName, allFeatures, params)
	  End Sub

	  Public Overridable WriteOnly Property SerializedModelFileName As String
		  Set(ByVal serializedModelFileName As String)
			Me.serializedModelFileName_Conflict = serializedModelFileName
		  End Set
	  End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void init(org.apache.solr.core.SolrResourceLoader solrResourceLoader) throws org.apache.solr.ltr.model.ModelException
	  Public Overrides Sub init(ByVal solrResourceLoader As SolrResourceLoader)
		MyBase.init(solrResourceLoader)
		Try
		  model = restoreModel(openInputStream())
		Catch e As Exception
		  Throw New ModelException("Failed to restore model from given file (" & serializedModelFileName_Conflict & ")", e)
		End Try
		validate()
	  End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.io.InputStream openInputStream() throws java.io.IOException
	  Protected Friend Overridable Function openInputStream() As Stream
		Return solrResourceLoader.openResource(serializedModelFileName_Conflict)
	  End Function

	  ''' <summary>
	  ''' Uses the <seealso cref="ModelGuesser.loadModelGuess(InputStream)"/> method.
	  ''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model restoreModel(java.io.InputStream inputStream) throws Exception
	  Protected Friend Overridable Function restoreModel(ByVal inputStream As Stream) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File instanceDir = solrResourceLoader.getInstancePath().toFile();
		Dim instanceDir As File = solrResourceLoader.getInstancePath().toFile()
		Return ModelGuesser.loadModelGuess(inputStream, instanceDir)
	  End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void validate() throws org.apache.solr.ltr.model.ModelException
	  Protected Friend Overrides Sub validate()
		MyBase.validate()
		If serializedModelFileName_Conflict Is Nothing Then
		  Throw New ModelException("no serializedModelFileName configured for model " & name)
		End If
		If model IsNot Nothing Then
		  validateModel()
		End If
	  End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void validateModel() throws org.apache.solr.ltr.model.ModelException
	  Protected Friend Overridable Sub validateModel()
		Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] mockModelFeatureValuesNormalized = new float[features.size()];
		  Dim mockModelFeatureValuesNormalized(features.size() - 1) As Single
		  score(mockModelFeatureValuesNormalized)
		Catch exception As Exception
		  Throw New ModelException("score(...) test failed for model " & name, exception)
		End Try
	  End Sub

	  Public Overrides Function score(ByVal modelFeatureValuesNormalized() As Single) As Single
		Return outputScore(model, modelFeatureValuesNormalized)
	  End Function

	  ''' <summary>
	  ''' Uses the <seealso cref="NetworkUtils.output(Model, INDArray)"/> method.
	  ''' </summary>
	  Public Shared Function outputScore(ByVal model As Model, ByVal modelFeatureValuesNormalized() As Single) As Single
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.create(new float[][]{ modelFeatureValuesNormalized });
		Dim input As INDArray = Nd4j.create(New Single()(){ modelFeatureValuesNormalized })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = org.deeplearning4j.util.NetworkUtils.output(model, input);
		Dim output As INDArray = NetworkUtils.output(model, input)
		Return output.getFloat(0)
	  End Function

	  Public Overrides Function explain(ByVal context As LeafReaderContext, ByVal doc As Integer, ByVal finalScore As Single, ByVal featureExplanations As IList(Of Explanation)) As Explanation

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
		Dim sb As New StringBuilder()

		sb.Append("(name=").Append(getName())
		sb.Append(",class=").Append(Me.GetType().Name)
		sb.Append(",featureValues=[")
		For i As Integer = 0 To featureExplanations.Count - 1
		  Dim featureExplain As Explanation = featureExplanations(i)
		  If i > 0 Then
			sb.Append(","c)
		  End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String key = features.get(i).getName();
		  Dim key As String = features.get(i).getName()
		  sb.Append(key).Append("="c).Append(featureExplain.getValue())
		Next i
		sb.Append("])")

		Return Explanation.match(finalScore, sb.ToString())
	  End Function

	End Class

End Namespace