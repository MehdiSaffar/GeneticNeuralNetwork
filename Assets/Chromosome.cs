using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class Chromosome {
	public List<double> genes = new List<double>();

	public static Chromosome RandomChromosome(int geneCount){
		var chromosome = new Chromosome();
		for (int i = 0; i < geneCount; i++) {
			chromosome.genes.Add(Random.Range(-1.0f, 1.0f));
		}
		return chromosome;
	}

	public static Chromosome Crossover(Chromosome first, Chromosome second){
		var genes = first.genes.Zip(second.genes, (x, y) => Random.value >= 0.5f ?  x : y).ToList();
		return new Chromosome{genes = genes};
	}

	public override string ToString() => "Genes: " + genes.ToPrettyString();
}
